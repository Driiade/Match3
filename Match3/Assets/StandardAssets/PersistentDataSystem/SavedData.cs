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


namespace BC_Solution
{
    [System.Serializable]
    public class SavedData
    {
        /// <summary>
        /// The current file path for this saved data.
        /// Null if no file is associed to the data (it is not already save in a file).
        /// Can change at any time.
        /// Do not modify this manually.
        /// </summary>
        [System.NonSerialized]
        public string filePath = null;

        /// <summary>
        /// The file number associed to this saved data.
        /// Can change at any time.
        /// Do not modify this manually.
        /// </summary>
        [System.NonSerialized]
        public int fileNumber = -1;

        public int dataVersion;

        public long timestamp;


        /// <summary>
        /// Function called when OnDatacreated is call by persistentData.
        /// This is fire when no data is detected in default saved data or in player saved data.
        /// </summary>
        /// <param name="dataVersion"></param>
        public virtual void OnDataCreated(int dataVersion) { } 

        /// <summary>
        /// Called when default saved data are loaded (when there is no player saved data and there is a default saved data file)
        /// </summary>
        public virtual void OnDefaultDataLoaded() { }
    }
}
