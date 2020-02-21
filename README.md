# apigateway
Let start by creating a simple Asp.net core api. This service is going to return a catalog in hierarchical structure. 
The source data is flattened and could be coming from a repository but for now it's hard coded:
```csharp
public class Category
{
    public string Key { get; set; }
    public string Label { get; set; }
    public string ParentCategory { get; set; }
}

private static readonly Category[] Categories = new[]
{
    new Category {Key = "automative", Label="Automative"},
    new Category {Key = "book", Label="Books"},
    new Category {Key = "fashion", Label="Clothing, Shoes & Accessories"},
    new Category {Key = "fashion-womens", Label="Women", ParentCategory = "fashion"},
    new Category {Key = "fashion-mens", Label="Men", ParentCategory = "fashion"},
    new Category {Key = "fashion-baby", Label="Baby", ParentCategory = "fashion"},
    new Category {Key = "electronic", Label="Electronic"},
    new Category {Key = "electronic-mobile", Label="Mobile", ParentCategory = "electronic"},
    new Category {Key = "electronic-mobile-accessories", Label="Accessories", ParentCategory = "electronic-mobile"},
    new Category {Key = "electronic-computer", Label="Computer", ParentCategory = "electronic"},
};

```
The output structure is hierarchical:
```csharp
public class CategoryViewModel
{
    [JsonPropertyName("id")]
    public string Key { get; set; }
    [JsonPropertyName("name")]
    public string Label { get; set; }
    public List<CategoryViewModel> Children { get; set; }
}
```
The API would return the Json like:
```json
[
  {"id":"automative","name":"Automative"},
  {"id":"book","name":"Books"},
  {"id":"fashion","name":"Clothing, Shoes & Accessories",
   "children":
   [
     {"id":"fashion-womens","name":"Women"},
     {"id":"fashion-mens","name":"Men"},
     {"id":"fashion-baby","name":"Baby"}
  ]},
  {"id":"electronic","name":"Electronic",
   "children":
   [
     {"id":"electronic-mobile","name":"Mobile",
      "children":
      [
        {"id":"electronic-mobile-accessories","name":"Accessories"}
      ]},
      {"id":"electronic-computer","name":"Computer"}
   ]
  }
]
```
I have added the swagger (The UI was not needed for this sample) and then published the API to Azure. 

## Creating Azure API management
The first step of creating an API management resource is straight forward but it's very time taking.
Then we would be with an *Echo API*
![image01](https://github.com/mkokabi/images/blob/master/AzureApiGateway/iamge01.png?raw=true)
The next step is adding our API. All we need is the URL to our *Open API* json.
![image02](https://github.com/mkokabi/images/blob/master/AzureApiGateway/iamge02.png?raw=true)
Then we would land on Design tab page. 
![image03](https://github.com/mkokabi/images/blob/master/AzureApiGateway/iamge03.png?raw=true)
The main 4 areas here are *Frontend*, *Inbound*, *Outbound* and *Backend*. The arrows show the flow of processing the request. 
Before being able to test our API we need to define the *Backend*. Click on the pen icon next to the *HTTP(s) endpoint*. Here, select the override and enter the service URL and hit *Save*
![image04](https://github.com/mkokabi/images/blob/master/AzureApiGateway/iamge04.png?raw=true)

Now we can test the API.
![image05](https://github.com/mkokabi/images/blob/master/AzureApiGateway/iamge05.png?raw=true)

The result after clicking on *Send* should be like:
![image06](https://github.com/mkokabi/images/blob/master/AzureApiGateway/iamge06.png?raw=true)

If we go back to *Design* and click on *</>* on *Backend* box (or on any other box), it would show the *Policies* for all the steps:
```xml
<policies>
    <inbound>
        <base />
        <set-backend-service base-url="https://catalogservicetest.azurewebsites.net/" />
    </inbound>
    <backend>
        <base />
    </backend>
    <outbound>
        <base />
    </outbound>
    <on-error>
        <base />
    </on-error>
</policies>
```
The change that we made (defining the back end service) is stored here. 

## Caching
Let's get back to the the designer and click on *+ Add Policy* on *Inbound* area. 
From the available policies for inbound select the *Cache*
![AddInBound](https://github.com/mkokabi/images/blob/master/AzureApiGateway/AddInbound.png?raw=true)
For simple cases we can specify just a time
![CahceBasic]
(https://github.com/mkokabi/images/blob/master/AzureApiGateway/cach-policy-basic.png?raw=true)
while for more advanced settings we can select *Full*
![CahceFull](https://github.com/mkokabi/images/blob/master/AzureApiGateway/cach-policy-full.png?raw=true)

Again after saving our changes we can review them in the policies panel:
```xml
<policies>
    <inbound>
        <base />
        <set-backend-service base-url="https://catalogservicetest.azurewebsites.net/" />
        <cache-lookup vary-by-developer="false" vary-by-developer-groups="false" downstream-caching-type="none" />
    </inbound>
    <backend>
        <base />
    </backend>
    <outbound>
        <base />
        <cache-store duration="300" />
    </outbound>
    <on-error>
        <base />
    </on-error>
</policies>
```
As we can see the changes are in both areas: *inbound* and *outbound*.

## CORS
To be able to call our API from javascript we need to allow our origin. For this sample we are going to allow our localhost, all methods, all headers, etc.
![CORS](https://github.com/mkokabi/images/blob/master/AzureApiGateway/COORSPolicy.png?raw=true)
Now our policy would be like:
```xml
<policies>
    <inbound>
        <base />
        <set-backend-service base-url="https://catalogservicetest.azurewebsites.net/" />
        <cache-lookup vary-by-developer="false" vary-by-developer-groups="false" downstream-caching-type="none" />
        <cors>
            <allowed-origins>
                <origin>http://localhost:3000/</origin>
            </allowed-origins>
            <allowed-methods>
                <method>*</method>
            </allowed-methods>
            <allowed-headers>
                <header>*</header>
            </allowed-headers>
            <expose-headers>
                <header>*</header>
            </expose-headers>
        </cors>
    </inbound>
    <backend>
        <base />
    </backend>
    <outbound>
        <base />
        <cache-store duration="300" />
    </outbound>
    <on-error>
        <base />
    </on-error>
</policies>
```

## Subscription
While we can disable this feature using the "Subscription required" checkbox in the *Settings* tab page of the API, but we want to quickly demonstrate this feature.
Go to the *Subscription* panel and create a new subscription:
![Subscription](https://github.com/mkokabi/images/blob/master/AzureApiGateway/AddSubscription.png?raw=true)

Get the subscription key:
![Subscription](https://github.com/mkokabi/images/blob/master/AzureApiGateway/GetSubKeys.png?raw=true)

and make sure in your code you are passing it in a header. The default header is *Ocp-Apim-Subscription-Key* but it could be defined in the *Settings* and it can be even passed as a query parameter which by default is *subscription-key*.
![SubscriptionSettings](https://github.com/mkokabi/images/blob/master/AzureApiGateway/SubscriptionSettings.png?raw=true)

The code would be like:
```javascript
  const requestOptions = {
    method: "GET",
    headers: {
      "Ocp-Apim-Subscription-Key": `${process.env.REACT_APP_BACKEND_API_SUBSCRIPTION_KEY}`
    }
  };

  useEffect(() => {
    fetch(`${process.env.REACT_APP_BACKEND_API_URL}/categories`,
    requestOptions)
    .then(receviedData => receviedData.text()
    .then(text => setTreeData(JSON.parse(text))));
  }, []);
```
The end result would look like:
![Final]
(https://github.com/mkokabi/images/blob/master/AzureApiGateway/FinalResult.png?raw=true)
