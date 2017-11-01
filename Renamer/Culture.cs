using System.Collections.Generic;
using System.Collections;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;

namespace Renamer
{
    public class Culture
    {
        public bool femaleSurnamesExist = false;
        public string cultureName = "";
        public string[] fnames1 = { };
        public string[] fnames2 = { };
        public string[] fnames3 = { };
        public string[] mnames1 = { };
        public string[] mnames2 = { };
        public string[] mnames3 = { };
        public string[] lnames1 = { };
        public string[] lnames2 = { };
        public string[] lnames3 = { };
        public string[] flnames1 = { };
        public string[] flnames2 = { };
        public string[] flnames3 = { };

        public Culture(ConfigNode node)
        {
            string[] vals;
            if (node.HasValue("name"))
            {
                cultureName = node.GetValue("name");
            }

            foreach (ConfigNode childNode in node.nodes)
            {
                vals = childNode.GetValues("key");
                if (vals.Length > 0)
                {
                    switch (childNode.name)
                    {
                        case "FFIRSTNAME1":
                            fnames1 = vals;
                            break;
                        case "FFIRSTNAME2":
                            fnames2 = vals;
                            break;
                        case "FFIRSTNAME3":
                            fnames3 = vals;
                            break;
                        case "MFIRSTNAME1":
                            mnames1 = vals;
                            break;
                        case "MFIRSTNAME2":
                            mnames2 = vals;
                            break;
                        case "MFIRSTNAME3":
                            mnames3 = vals;
                            break;
                        case "LASTNAME1":
                            lnames1 = vals;
                            break;
                        case "LASTNAME2":
                            lnames2 = vals;
                            break;
                        case "LASTNAME3":
                            lnames3 = vals;
                            break;
                        case "FLASTNAME1":
                            flnames1 = vals;
                            break;
                        case "FLASTNAME2":
                            flnames2 = vals;
                            break;
                        case "FLASTNAME3":
                            flnames3 = vals;
                            break;
                        default:
                            break;
                    }

                    if (childNode.name.StartsWith("FLASTNAME"))
                    {
                        femaleSurnamesExist = true;
                    }
                }
            }
        }
    }
}
