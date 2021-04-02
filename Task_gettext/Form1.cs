using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Task_gettext
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = openFileDialog1.FileName;
            XDocument xDoc = CreateXml(filename);
            FillTreeView(xDoc, treeView1);
        }
        XDocument CreateXml(string filename)
        {
            XDocument xDoc = new XDocument();
            XElement root = new XElement("root");
            xDoc.Add(root);

            using (StreamReader sr = new StreamReader(filename))
            {
                XElement msg = new XElement("msg");
                XElement currentElement = root;
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                    if (line.StartsWith("msgctxt"))
                    {
                        string[] ctxArray = line.Substring(line.IndexOf("\"") + 1).TrimEnd('\"').Split('.');
                        foreach (string ctx in ctxArray)
                        {
                            if (!currentElement.Elements(ctx).Any())
                            {
                                currentElement.Add(new XElement(ctx));
                            }
                            currentElement = currentElement.Elements(ctx).FirstOrDefault();
                        }
                    }
                    if (line.StartsWith("msgid"))
                    {
                        XAttribute id = new XAttribute("id", line.Substring(line.IndexOf("\"") + 1).TrimEnd('\"'));
                        msg.Add(id);
                    }
                    if (line.StartsWith("msgstr"))
                    {
                        XAttribute str = new XAttribute("str", line.Substring(line.IndexOf("\"") + 1).TrimEnd('\"'));
                        msg.Add(str);
                        currentElement.Add(msg);
                        msg = new XElement("msg");
                        currentElement = root;
                    }
                }
            }
            return xDoc;
        }
        private void FillTreeView(XDocument xDoc, TreeView trv)
        {
            XElement rootXml = xDoc.Root;
            trv.Nodes.Clear();        
            MsgTreeNode rootTrv = new MsgTreeNode();
            rootTrv.Text = rootXml.Name.LocalName;
            trv.Nodes.Add(rootTrv);
            AddTreeViewChildNodes(rootTrv, rootXml);
        }

        private void AddTreeViewChildNodes(MsgTreeNode parentNodeTrv, XElement nodeXml)
        {
            foreach (XElement childNodeTrv in nodeXml.Elements())
            {
                if (childNodeTrv.Name != "msg")
                {
                    MsgTreeNode newNode = new MsgTreeNode();
                    newNode.Text = childNodeTrv.Name.LocalName;
                    parentNodeTrv.Nodes.Add(newNode);

                    AddTreeViewChildNodes(newNode, childNodeTrv);
                    if (newNode.Nodes.Count == 0) newNode.EnsureVisible();
                }
                else
                {
                    parentNodeTrv.msgs.Add(new Msg(childNodeTrv.Attribute("id").Value, childNodeTrv.Attribute("str").Value));
                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            textBox1.Clear();
            MsgTreeNode node = (MsgTreeNode)e.Node;
            textBox1.Lines = node.msgs.Select(x => x.id).ToArray();
        }
    }

    public class Msg
    {
        public string id;
        public string str;
        public Msg(string id, string str)
        {
            this.id = id;
            this.str = str;
        }
    }
    public class MsgTreeNode : TreeNode
    {
        public List<Msg> msgs = new List<Msg>();
    }
}
