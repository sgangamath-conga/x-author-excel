/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;

/*
 * Class to hold error row highlighting data. Backed by a dictionary of dictionary. Each key is a range in A1Notation. The value will be a dictionry<string, string>.
 * The key for the inner dictionary is for a property and the value is the value of that key. A range must first be created using createNewRange. Afterwards the 
 * getters and setters may be used. 
 * The current properties used are:
 * "color"
 * "colNum"
 * 
 */
namespace Apttus.XAuthor.Core
{
    public class RowHighLighting
    {
        private Dictionary<string, Dictionary<string, string >> highLightDict;

        /// <summary>
        /// key: workflowId, value: list of ranges. 
        /// dictionary that stores a list of ranges highlighted/commented with the workflowId as its key.
        /// </summary>
        private Dictionary<string, HashSet<string>> workflowToRange;

        /// <summary>
        /// constructor
        /// </summary>
        public RowHighLighting()
        {
            highLightDict = new Dictionary<string, Dictionary<string, string>>();
            workflowToRange = new Dictionary<string, HashSet<string>>();
        }
        /// <summary>
        /// clears all internal dictionaries
        /// </summary>
        public void Clear()
        {
            highLightDict.Clear();
            workflowToRange.Clear();
        }

        /// <summary>
        /// clears the internal dictionaries based on the supplied workflowId
        /// </summary>
        /// <param name="workflowId"></param>
        public void Clear(string workflowId)
        {
            if(string.IsNullOrEmpty(workflowId))
            {
                return;
            }
            else
            {
                Dictionary<string, string> values;
                if(highLightDict.TryGetValue(workflowId, out values))
                {
                    values.Clear();
                }
                HashSet<string> rangeSet;
                if (workflowToRange.TryGetValue(workflowId, out rangeSet))
                {
                    rangeSet.Clear();
                }

            }
        }

        /// <summary>
        /// bool to track if highlights have been cleared for the current workflow
        /// </summary>
        public Boolean isCleared = false;
        /// <summary>
        /// gets number of records saved with error data 
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return highLightDict.Count;
        }

        /// <summary>
        /// Creates a placeholder dictionary for the given range
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public bool CreateNewRange(string range, string sMapId)
        {
            if (!string.IsNullOrEmpty(range) && !string.IsNullOrEmpty(sMapId))
            {
                Dictionary<string, string> values;
                bool success = highLightDict.TryGetValue(range, out values);
                if (!success)//if range is not already there, create new dictionary            
                {
                    highLightDict.Add(range, new Dictionary<string, string>());
                    HashSet<string> rangeSet;
                    if (workflowToRange.TryGetValue(sMapId, out rangeSet))
                    {
                        rangeSet.Add(range);
                    }
                    else //first time, create new set and add range
                    {
                        rangeSet = new HashSet<string>();
                        rangeSet.Add(range);
                        workflowToRange.Add(sMapId, rangeSet);
                    }
                    return true;
                }
                else //range already exists and may be used in multiple save maps
                {
                    HashSet<string> rangeSet;
                    bool repeatMap = workflowToRange.TryGetValue(sMapId, out rangeSet);
                    if (!repeatMap) //new sMap, but same range
                    {
                        rangeSet = new HashSet<string>();
                        rangeSet.Add(range);
                        workflowToRange.Add(sMapId, rangeSet);
                    }else if (workflowToRange.ContainsKey(sMapId))
                    {
                        workflowToRange[sMapId].Add(range);
                    }else
                    {
                        workflowToRange.Remove(sMapId);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// range in A1 notation, adds range to list of ranges associated with the given workflowId
        /// </summary>
        /// <param name="range"></param>
        /// <param name="sMapId"></param>
        public void AddRangeToWorkflowId(string range, string sMapId)
        {
            HashSet<string> rangeSet = new HashSet<string>();
            if (workflowToRange.TryGetValue(range, out rangeSet))
            {
                rangeSet.Add(range);
            }
        }

        /// <summary>
        /// range in a1 notation, color represented as a double. Sets the color for the given range
        /// </summary>
        /// <param name="range"></param>
        /// <param name="color"></param>
        public void SetColor(string range, double color)
        {
            Dictionary<string, string> values;
            bool success = highLightDict.TryGetValue(range, out values);
            string currentColor = "";
            success = values.TryGetValue("color", out currentColor);
            if (success) //if range already has a color setting
            {
                values["color"] = color.ToString();
            }
            else //create new key
            {
                values.Add("color", color.ToString());
            }
        }
        /// <summary>
        /// range in a1 notation, colNum as an integer. Sets the comment colnumber for a given range. 
        /// </summary>
        /// <param name="range"></param>
        /// <param name="colNum"></param>
        public void SetCommentCol(string range, int colNum)
        {
            Dictionary<string, string> values;
            bool success = highLightDict.TryGetValue(range, out values);
            string commentColNum = "";
            success = values.TryGetValue("colNum", out commentColNum);
            if (success)
            {
                values["colNum"] = colNum.ToString();
            }
            else
            {
                values.Add("colNum", colNum.ToString());
            }
        }

        /// <summary>
        /// returns the color (as a double) for a given range. returns -1 if color is not set
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public double GetColor(string range)
        {
            Dictionary<string, string> values;
            bool success = highLightDict.TryGetValue(range, out values);            
            string previousColor = "";
            success = values.TryGetValue("color", out previousColor);
            if(success)
            {
                return Convert.ToDouble(previousColor);
            }
            return -1;
        }
        /// <summary>
        /// returns the comment column number. returns -1 if colnum is not set
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public int GetColNum(string range)
        {
            Dictionary<string, string> values;
            bool success = highLightDict.TryGetValue(range, out values);
            string colNum = "";
            success = values.TryGetValue("colNum", out colNum);
            if(success)
            {
                return Convert.ToInt32(colNum);
            }
            return -1;
        }
        /// <summary>
        /// returns a list of ranges. Used for iterating through through each range that has been highlighted. 
        /// </summary>
        /// <returns></returns>
        public List<string> GetRanges()
        {
            return highLightDict.Keys.ToList();
        }

        /// <summary>
        /// returns a list of ranges asociated with the given workflowId
        /// </summary>
        /// <param name="sMapId"></param>
        /// <returns></returns>
        public HashSet<string> GetRangesBysMapId(string sMapId)
        {
            HashSet<string> rangeSet = new HashSet<string>();
            if(string.IsNullOrEmpty(sMapId))
            {
                return rangeSet;
            }
            else
            {
                if (workflowToRange.TryGetValue(sMapId, out rangeSet))
                {
                    return rangeSet;
                }
            }
            return rangeSet;
        }

    }
}
