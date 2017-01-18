using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;


namespace JonesNicholson
{
    
    /// <remarks>
   /// This application's main class. The class must be Public.
   /// </remarks>
   public class CsAddPanel : IExternalApplication
   {
      // Both OnStartup and OnShutdown must be implemented as public method
      public Result OnStartup(UIControlledApplication application)
      {
         // Add a new ribbon panel
         RibbonPanel ribbonPanel = application.CreateRibbonPanel("JONES NICHOLSON");

         // Create a push button to trigger a command add it to the ribbon panel.
         // Adding lining to duct
         string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;
         PushButtonData buttonData = new PushButtonData("cmdAddLining",
            "ADD LINING", thisAssemblyPath, "JonesNicholson.AddLiningToDuct");

         PushButton pushButton = ribbonPanel.AddItem(buttonData) as PushButton;
         TextBoxData itemData3 = new TextBoxData("itemName3");
         Autodesk.Revit.UI.TextBox item3 = ribbonPanel.AddItem(itemData3) as Autodesk.Revit.UI.TextBox;
         item3.Value = "Option 3";
         item3.ToolTip = itemData3.Name; // Can be changed to a more descriptive text. 

         // Optionally, other properties may be assigned to the button
         // a) tool-tip
         pushButton.ToolTip = "Will add lining to duct , keeping free area size intact";

         // b) large bitmap
         Uri uriImage = new Uri(@"C:\Users\Alext\OneDrive\Documents\Visual Studio 2013\Projects\Revit API\JonesNicholson\JonesNicholson\bin\Debug\39_globe_72x72.png");
         BitmapImage largeImage = new BitmapImage(uriImage);
         pushButton.LargeImage = largeImage;

         // Create a push button to trigger a command add it to the ribbon panel.
         // At this stage this button is for demonstration purposes , no functionality is implemented
     
         PushButtonData buttonDataTest = new PushButtonData("cmdRemoveLining",
            "Remove LINING", thisAssemblyPath, "JonesNicholson.RemoveLiningToDuct");


         PushButton pushButtonTest = ribbonPanel.AddItem(buttonDataTest) as PushButton;

         // Optionally, other properties may be assigned to the button
         // a) tool-tip
         pushButton.ToolTip = "Will add lining to duct , keeping free area size intact";

         // b) large bitmap
       
         pushButtonTest.LargeImage = largeImage;

         return Result.Succeeded;
      }

      public static bool CollectDataInput(string title, out int ret)
      {
          Form dc = new Form();
          dc.he = title;

          dc.HelpButton = dc.MinimizeBox = dc.MaximizeBox = false;
          dc.ShowIcon = dc.ShowInTaskbar = false;
          dc.TopMost = true;

          dc.Height = 100;
          dc.Width = 300;
          dc.MinimumSize = new Size(dc.Width, dc.Height);

          int margin = 5;
          Size size = dc.ClientSize;

          TextBox tb = new TextBox();
          tb.
          tb.TextAlign = HorizontalAlignment.Right;
          tb.Height = 20;
          tb.Width = size.Width - 2 * margin;
          tb.Location = new Point(margin, margin);
          tb.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
          dc.Controls.Add(tb);

          Button ok = new Button();
          ok.Text = "Ok";
          ok.Click += new EventHandler(ok_Click);
          ok.Height = 23;
          ok.Width = 75;
          ok.Location = new Point(size.Width / 2 - ok.Width / 2, size.Height / 2);
          ok.Anchor = AnchorStyles.Bottom;
          dc.Controls.Add(ok);
          dc.AcceptButton = ok;

          dc.ShowDialog();

          return int.TryParse(tb.Text, out ret);
      }

      public Result OnShutdown(UIControlledApplication application)
      {
         // nothing to clean up in this simple case
         return Result.Succeeded;
      }
   }
   /// <remarks>
   /// The "HelloWorld" external command. The class must be Public.
   /// </remarks>
   /// 


   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   public class RemoveLiningToDuct : IExternalCommand
   {
       // The main Execute method (inherited from IExternalCommand) must be public
       public Autodesk.Revit.UI.Result Execute(ExternalCommandData revit,
           ref string message, ElementSet elements)
       {
           TaskDialog.Show("Revit", "Under Construction");
           return Autodesk.Revit.UI.Result.Succeeded;
       }
   }



 [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   public class AddLiningToDuct : IExternalCommand
   {
      // The main Execute method (inherited from IExternalCommand) must be public
     public Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData,
          ref string message, ElementSet elements)
      {
          try
          {
              // Select some elements in Revit before invoking this command

              // Get the handle of current document.
              UIDocument uidoc = commandData.Application.ActiveUIDocument;
              Document doc = uidoc.Document;
              // Get the element selection of current document.
              Selection selection = uidoc.Selection;
              ICollection<ElementId> selectedIds = uidoc.Selection.GetElementIds();

              if (0 == selectedIds.Count)
              {
                  // If no elements selected.
                  TaskDialog.Show("Revit", "You haven't selected anything.");
              }
              else
              {
                  String info = "Ids of selected elements in the document area: ";
                  foreach (ElementId id in selectedIds)
                  {
                      info += "\n\t" + id.IntegerValue;
                      Element duct = uidoc.Document.GetElement(id);
                      TaskDialog.Show("Failure", "Debug 1: It gets here ");
                      // Check to see if element is a duct

                      if (duct.GetType().ToString().Equals("Autodesk.Revit.DB.Mechanical.Duct"))
                      {
                          TaskDialog.Show("Failure", duct.GetType().ToString());
                          
                          Parameter Width = duct.get_Parameter(BuiltInParameter.RBS_CURVE_WIDTH_PARAM);
                          double W = Width.AsDouble();
                          Parameter Height = duct.get_Parameter(BuiltInParameter.RBS_CURVE_HEIGHT_PARAM);
                          double H = Height.AsDouble();
                          using (Transaction tx = new Transaction(doc))
                          {
                              if (tx.Start("Create model curves") == TransactionStatus.Started)
                              {
                                  Width.Set(W * 2);
                                  Height.Set(H * 2);
                                  //  List<ElementId> ids = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_DuctCurves).ToElementIds().ToList();
                                  //    lining.Set(Lin * 2);
                                  if (TransactionStatus.Committed != tx.Commit())
                                  {
                                      TaskDialog.Show("Failure", "Transaction could not be committed");
                                  }
                              }
                              else
                              {
                                  tx.RollBack();
                              }
                          }

                      }
                      
                     
                  }

                  TaskDialog.Show("Revit", info);
              }
          }
          catch (Exception e)
          {
              message = e.Message;
              return Autodesk.Revit.UI.Result.Failed;
          }

          return Autodesk.Revit.UI.Result.Succeeded;
      }

   }


   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.ReadOnly)]
   public class Document_Selection : IExternalCommand
   {
       public Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData,
           ref string message, ElementSet elements)
       {
           try
           {
               // Select some elements in Revit before invoking this command

               // Get the handle of current document.
               UIDocument uidoc = commandData.Application.ActiveUIDocument;

               // Get the element selection of current document.
               Selection selection = uidoc.Selection;
               ICollection<ElementId> selectedIds = uidoc.Selection.GetElementIds();

               if (0 == selectedIds.Count)
               {
                   // If no elements selected.
                   TaskDialog.Show("Revit", "You haven't selected any elements.");
               }
               else
               {
                   String info = "Ids of selected elements in the document are: ";
                   foreach (ElementId id in selectedIds)
                   {
                       info += "\n\t" + id.IntegerValue;
                   }

                   TaskDialog.Show("Revit", info);
               }
           }
           catch (Exception e)
           {
               message = e.Message;
               return Autodesk.Revit.UI.Result.Failed;
           }

           return Autodesk.Revit.UI.Result.Succeeded;
       }
       /// </ExampleMethod>
   }



   class MyClass //Notice the class 
   {
       void GetElementParameterInformation(Document document, Element element)
       {
           // Format the prompt information string
           String prompt = "Show parameters in selected Element: \n\r";

           StringBuilder st = new StringBuilder();
           // iterate element's parameters
           foreach (Parameter para in element.Parameters)
           {
               st.AppendLine(GetParameterInformation(para, document));
           }

           // Give the user some information
           TaskDialog.Show("Revit", prompt + st.ToString());
       }

       String GetParameterInformation(Parameter para, Document document)
       {
           string defName = para.Definition.Name + "\t : ";
           string defValue = string.Empty;
           // Use different method to get parameter data according to the storage type
           switch (para.StorageType)
           {
               case StorageType.Double:
                   //covert the number into Metric
                   defValue = para.AsValueString();
                   break;
               case StorageType.ElementId:
                   //find out the name of the element
                   Autodesk.Revit.DB.ElementId id = para.AsElementId();
                   if (id.IntegerValue >= 0)
                   {
                       defValue = document.GetElement(id).Name;
                   }
                   else
                   {
                       defValue = id.IntegerValue.ToString();
                   }
                   break;
               case StorageType.Integer:
                   if (ParameterType.YesNo == para.Definition.ParameterType)
                   {
                       if (para.AsInteger() == 0)
                       {
                           defValue = "False";
                       }
                       else
                       {
                           defValue = "True";
                       }
                   }
                   else
                   {
                       defValue = para.AsInteger().ToString();
                   }
                   break;
               case StorageType.String:
                   defValue = para.AsString();
                   break;
               default:
                   defValue = "Unexposed parameter.";
                   break;
           }

           return defName + defValue;
       }


   }



}


