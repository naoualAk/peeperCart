using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Drawing;

namespace SegmentationCreator
{
    public class ExcelWriter
    {

        //Excel workbook
        private Workbook workbook;
        //Excel worksheet
        private static Worksheet worksheet;
        //Application
        private Application app;
        //Excel range
        private static Range range;
        //Excel file path
        private string path;
        //Excel line counter
        private static int lineCounter;

        public ExcelWriter()
        {
            workbook = null;
            worksheet = null;
            range = null;
            lineCounter = 1;
        }


        public ExcelWriter(String path)
        {
            this.path = path;
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                //Create a new worksheet
                app = new Application();
                workbook = app.Workbooks.Add(System.Reflection.Missing.Value);
                worksheet = (Worksheet)workbook.Worksheets.get_Item(1);
                range = (Range)worksheet.get_Range("1:1");
            }
            catch
            {
                throw new Exception("Cannot create/overwrite the file [" + path + "].\n Does it already opened?");
            }
            lineCounter = 1;
        }

        public void Write<T>(T list)
        {
           worksheet.Cells[lineCounter,1] = list;
           lineCounter++;
        }

        public void WriteStats<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
                worksheet.Cells[lineCounter, i+1] = list[i];

            lineCounter++;
        }

        public void WriteStats<T>(List<T> list, Color c)
        {
            for (int i = 0; i < list.Count; i++)
            {
                worksheet.Cells[lineCounter, i + 1] = list[i];
                worksheet.Cells[lineCounter, i + 1].Interior.Color = c;
            }

            lineCounter++;
        }

        public void Save()
        {
            try
            {
                //Save the file in the path 
                workbook.SaveAs(path, XlFileFormat.xlWorkbookDefault,
                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlExclusive,
                            XlSaveConflictResolution.xlLocalSessionChanges, Type.Missing, Type.Missing);
                workbook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
                //Destroy Excel objects
                app.Quit();
                ReleaseObject(worksheet);
                ReleaseObject(workbook);
                ReleaseObject(app);
            }
            catch
            {
                throw new Exception("Cannot save Excel file [" + path + "]");
            }
        }

        public void Dispose()
        {
            try
            {
                workbook.Close(false, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
                app.Quit();
                ReleaseObject(worksheet);
                ReleaseObject(workbook);
                ReleaseObject(app);
            }
            catch{  }
        }

        private void ReleaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch
            {
                obj = null;
                throw new Exception("Cannot save Excel file [" + path + "]");
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
