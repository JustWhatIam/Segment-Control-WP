Segment-Control-WP
==================

Using radio button to implement, then it's welcome to refactor or make it in good coding form

MButton.DataSource = new List<MultipleButtonItem>() { 
  new MultipleButtonItem() { DisplayText = "123" }, 
  new MultipleButtonItem() { DisplayText = "Object" }, 
  new MultipleButtonItem() { DisplayText = "C#" } 
};
  
MButton.SelectionChanged += MButton_SelectionChanged;
  
