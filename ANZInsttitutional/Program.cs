using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANZInsttitutional
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter csv file path:");
            string csvpath= Console.ReadLine();

            List<nodes> liNodes = ReadNodes(csvpath);
            List<nodes> parents = (from nodes in liNodes
                          where string.IsNullOrEmpty(nodes.parent)
                          select nodes).ToList();

            foreach (var parent in parents)
            {
                string strNode = parent.entity;
                int totalUtilization = parent.utilisation;
                int limit = parent.limit;

                //recursive function for muti-level tree traversing
                nodeTraversing(parent, liNodes,ref totalUtilization,ref strNode);

                Console.WriteLine(string.Format("entities:{0}:", strNode));
                string breach = string.Empty;
                if (limit > totalUtilization)
                    breach = "No limit breaches";
                else
                    breach = string.Format("Limit breach at {0} (limit = {1}, direct utilisation = {2}, combined utilisation = {3}).",
                        parent.entity, parent.limit,parent.utilisation,totalUtilization);
                Console.WriteLine(breach);
                Console.ReadKey();
            }            

        }

        static void nodeTraversing(nodes child, List<nodes> liNodes,ref int totalUtilization,ref string strNode)
        {
                List<nodes> liSubChild = (from nodes in liNodes
                                          where nodes.parent.Equals(child.entity)
                                          select nodes).ToList();

                if (liSubChild.Count > 0)
                {
                    totalUtilization = totalUtilization + liSubChild.Sum(x => x.utilisation);
                    string entity = string.Join("/", liSubChild.Select(s => string.Format("{0}", s.entity)));
                    strNode = string.Format("{0}/{1}", strNode, entity); 
                }

            foreach (var subChild in liSubChild)
                nodeTraversing(subChild, liNodes, ref totalUtilization, ref strNode);

        }

        static List<nodes> ReadNodes(string path)
        {
            List<nodes> liNodes = new List<nodes>();
            try
            {
                using (TextFieldParser csvParser = new TextFieldParser(path))
                {
                    csvParser.CommentTokens = new string[] { "#" };
                    csvParser.SetDelimiters(new string[] { "," });
                    csvParser.HasFieldsEnclosedInQuotes = true;

                    // Skip the row with the column names
                    csvParser.ReadLine();

                    while (!csvParser.EndOfData)
                    {
                        // Read current line fields, pointer moves to the next line.
                        string[] fields = csvParser.ReadFields();
                        nodes objnode = new nodes();
                        objnode.entity = fields[0];
                        objnode.parent = fields[1];
                        objnode.limit = Convert.ToInt32(fields[2]);
                        objnode.utilisation = Convert.ToInt32(fields[3]);
                        liNodes.Add(objnode);
                    }

                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return liNodes;
        }
    }
}
