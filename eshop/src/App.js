import React, { useState, useEffect } from "react";
import { makeStyles } from '@material-ui/core/styles';
import TreeView from '@material-ui/lab/TreeView';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import ChevronRightIcon from '@material-ui/icons/ChevronRight';
import TreeItem from '@material-ui/lab/TreeItem';

const data = {
  id: "root",
  name: "Parent",
  children: [
    {
      id: "1",
      name: "Child - 1"
    },
    {
      id: "3",
      name: "Child - 3",
      children: [
        {
          id: "4",
          name: "Child - 4"
        }
      ]
    }
  ]
};

const useStyles = makeStyles({
  root: {
    height: 216,
    flexGrow: 1,
    maxWidth: 400
  }
});

function App() {
  const requestOptions = {
    method: "GET",
    headers: {
      "Ocp-Apim-Subscription-Key": `${process.env.REACT_APP_BACKEND_API_SUBSCRIPTION_KEY}`
    }
  };

  const classes = useStyles();
  const [treeData, setTreeData] = useState(null);
  useEffect(() => {
    fetch(`${process.env.REACT_APP_BACKEND_API_URL}/categories`,
    requestOptions)
    .then(receviedData => receviedData.text()
    .then(text => setTreeData(JSON.parse(text))));
  }, []);

  const renderTree = nodes => (
    Array.isArray(nodes)
    ? nodes.map(node => renderTreeItem(node))
    : null
  )

  const renderTreeItem = nodes => (
    <TreeItem key={nodes.id} nodeId={nodes.id} label={nodes.name}>
      {Array.isArray(nodes.children)
        ? nodes.children.map(node => renderTreeItem(node))
        : null}
    </TreeItem>
  );

  return (
    <div className="App">
      {treeData && 
      <TreeView
        className={classes.root}
        defaultCollapseIcon={<ExpandMoreIcon />}
        defaultExpanded={["root"]}
        defaultExpandIcon={<ChevronRightIcon />}
      >
        {renderTree(treeData)}
      </TreeView>}
    </div>
  );
}

export default App;
