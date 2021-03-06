/*Copyright(c) <2017> <Benoit Constantin ( France ) >

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. 
*/

using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace BC_Solution
{
    /// <summary>
    /// Because i'm bored of creating GUIStyle EVERYWHERE
    /// </summary>
    public class ScriptableGUIStyle : ScriptableObject
    {

        public enum BASE_SKIN { NONE, BUTTON, LABEL, TOGGLE, FOLDOUT, BOX, POPUP }

        [SerializeField]
        private BASE_SKIN baseSkin;

        [SerializeField]
        private int fontSize = 10;
        [SerializeField]
        private FontStyle fontStyle;

        [SerializeField]
        private TextAnchor textAnchor = TextAnchor.UpperLeft;

        [Space(10)]
        [SerializeField]
        private Color normalTextColor;

        private GUIStyle Load()
        {
            GUIStyle guiStyle = null;
            switch (baseSkin) 
            {
                case BASE_SKIN.BUTTON: guiStyle = new GUIStyle(GUI.skin.button); break;
                case BASE_SKIN.LABEL: guiStyle = new GUIStyle(GUI.skin.label); break;
                case BASE_SKIN.TOGGLE: guiStyle = new GUIStyle(GUI.skin.toggle); break;
                case BASE_SKIN.FOLDOUT: guiStyle = new GUIStyle(EditorStyles.foldout); break;
                case BASE_SKIN.BOX: guiStyle = new GUIStyle(GUI.skin.box); break;
                case BASE_SKIN.POPUP: guiStyle = new GUIStyle(EditorStyles.popup); break;
                case BASE_SKIN.NONE: guiStyle = new GUIStyle(); break;
            }

            guiStyle.name = this.name;

            guiStyle.fontSize = fontSize;
            guiStyle.fontStyle = fontStyle;

            guiStyle.normal.textColor = normalTextColor;
            guiStyle.alignment = textAnchor;

            return guiStyle;
        }

        private static bool hasLoadedStyle = false;
        /// <summary>
        /// Load all styles and place them in GUI.skin.customerStyles
        /// </summary>
        public static void LoadAllGUIStyle()
        {
            if (hasLoadedStyle)
                return;

            hasLoadedStyle = true;

            List<GUIStyle> styleToAdd = new List<GUIStyle>();
            ScriptableGUIStyle[] styles = FindAssetExtension.GetAllInstances<ScriptableGUIStyle>();


            GUIStyle[] registeredStyles = GUI.skin.customStyles;

            for (int i = 0; i < styles.Length; i++)
            {
                GUIStyle style = styles[i].Load();
                bool alreadyExisting = false;
                for (int j = 0; j < GUI.skin.customStyles.Length; j++)
                {
                    if (GUI.skin.customStyles[j].name == style.name)
                    {
                        registeredStyles[j] = style;

                        alreadyExisting = true;
                        break;
                    }
                }

                if(!alreadyExisting)
                {
                    styleToAdd.Add(style);
                }
            }

            int previousLength = registeredStyles.Length;
            Array.Resize(ref registeredStyles, registeredStyles.Length + styleToAdd.Count);
            for (int i = previousLength; i < (previousLength) + styleToAdd.Count; i++)
            {
                registeredStyles[i] = styleToAdd[i - previousLength];
            }
            GUI.skin.customStyles = registeredStyles;
        }
    }
}
