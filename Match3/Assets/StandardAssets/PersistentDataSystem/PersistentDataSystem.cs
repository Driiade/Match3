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

using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;

namespace BC_Solution
{
    public class PersistentDataSystem : MonoBehaviour
    {
        public enum AwakeLoadMode { NONE, SPECIFIC_CLASS, ALL_SAVED_DATA };
        public enum SaveMode { SINGLE_FILE, MULTIPLE_FILE };
        public enum PathMode { DEFAULT, PLAYER };
        /// <summary>
        /// A directory for storing all PersistentDataSystem files0
        /// </summary>
        public const string PersistentDataSystemDirectory = "PersistentDataSystem";

        /// <summary>
        /// The directory name used when automatic naming is using
        /// </summary>
        public const string AutomaticDirectoryName = "Automatic";

        /// <summary>
        /// The directory where is stocked the file for single file mode
        /// </summary>
        public const string SingleFileDirectoryName = "SingleFile";

        /// <summary>
        /// The name a the file used for the one file mode
        /// </summary>
        public const string SingleFileName = "savedData";

        /// <summary>
        /// The directory where is stocked the file for multiple files mode
        /// </summary>
        public const string MultipleFilesDirectoryName = "MultipleFiles";

        /// <summary>
        /// Extension of file that the system use for automatic (ie use of BinaryFormatter) serialization
        /// </summary>
        public const string AutomaticSerializationFileExtension = ".apds";

        /// <summary>
        /// Extension of file that the system use for controlled (ie use of StreamWriter) serialization
        /// </summary>
        public const string ControlledSerializationFileExtension = ".cpds";


        /// <summary>
        /// Called just before a data is saved to file.
        /// </summary>
        public Action<PersistentDataSystem, SavedData> OnBeginDataSaving;


        /// <summary>
        /// Called just after  a data is saved to file.
        /// </summary>
        public Action<PersistentDataSystem, SavedData> OnDataSaved;

        /// <summary>
        /// Called when the data is loaded from file/
        /// </summary>
        public Action<PersistentDataSystem, SavedData> OnDataLoaded;


        [Tooltip("Indentify the version of the persistentData")]
        public int dataVersion = 0;


        public bool autoSave = true;

        [Tooltip("The saved data loaded on awake")]
        public AwakeLoadMode awakeLoadMode = AwakeLoadMode.ALL_SAVED_DATA;

        [Tooltip("Lot of advantage :\n1- Saved data add-on can be done\n2- Partial data loading\n3- One file corruption do not impact the entire saved data")]
        public SaveMode saveMode = SaveMode.MULTIPLE_FILE;


        [Tooltip("Class to load at the start of the game")]
        public List<string> classToLoad;

        [Space(20)]
        [NonSerialized]
        public Dictionary<System.Type, List<SavedData>> savedDataDictionnary = new Dictionary<System.Type, List<SavedData>>();

        /// <summary>
        /// For automatic saving (for the moment we only use this)
        /// </summary>
        public string automaticPlayerSavedDataDirectoryPath { get; private set; }
        public string singlePlayerFileDirectoryPath { get; private set; }
        public string multiplePlayerFilesDirectoryPath { get; private set; }

        /// <summary>
        /// For saving default savedData
        /// </summary>
        public string singleDefaultFileDirectoryPath { get; private set; }
        public string multipleDefaultFilesDirectoryPath { get; private set; }

        private bool isInit = false;
        public bool IsInit
        {
            get { return isInit; }
        }


        void Awake()
        {
            if (!isInit)
                Init();

            switch (awakeLoadMode)
            {
                case AwakeLoadMode.SPECIFIC_CLASS: LoadClass(this.classToLoad); break;
                case AwakeLoadMode.ALL_SAVED_DATA: LoadAllSavedData(); break;
                default: break;
            }
        }

        /// <summary>
        /// Init the PersistentData system, important for the editor, used in the Awake function
        /// </summary>
        public void Init()
        {
#if UNITY_TVOS
            automaticPlayerSavedDataDirectoryPath = System.IO.Path.Combine(System.IO.Path.Combine(Application.temporaryCachePath, PersistentDataSystemDirectory), AutomaticDirectoryName);
#else
            automaticPlayerSavedDataDirectoryPath = System.IO.Path.Combine(System.IO.Path.Combine(Application.persistentDataPath, PersistentDataSystemDirectory), AutomaticDirectoryName);
#endif
            singlePlayerFileDirectoryPath = System.IO.Path.Combine(automaticPlayerSavedDataDirectoryPath, SingleFileName);
            multiplePlayerFilesDirectoryPath = System.IO.Path.Combine(automaticPlayerSavedDataDirectoryPath, MultipleFilesDirectoryName);

            singleDefaultFileDirectoryPath = System.IO.Path.Combine(System.IO.Path.Combine(Application.streamingAssetsPath, PersistentDataSystemDirectory), SingleFileName);
            multipleDefaultFilesDirectoryPath = System.IO.Path.Combine(System.IO.Path.Combine(Application.streamingAssetsPath, PersistentDataSystemDirectory), MultipleFilesDirectoryName);

            isInit = true;
        }


        void Reset()
        {
            OnBeginDataSaving = null;
            OnDataLoaded = null;

            if (classToLoad != null)
                classToLoad.Clear();

            if (savedDataDictionnary != null)
                savedDataDictionnary.Clear();
        }

        /// <summary>
        /// Create new instance of class and load it with the LoadSavedData function. Independent of multiple files or not.
        /// Will "destroy" all previous loaded data from persistentData dictionnary
        /// </summary>
        /// <param name="classToLoad"></param>
        /// <param name="pathMode">if defined on LoadMode.Default, it will load default saved data in the persistentData folder</param>
        /// <returns></returns>
        public bool LoadClass(List<string> classToLoad, PathMode pathMode = PathMode.PLAYER)
        {
            if (classToLoad == null)
            {
                Debug.LogError("Class empty : can not load data");
                return false;
            }

            savedDataDictionnary.Clear();

            if (saveMode == SaveMode.SINGLE_FILE)
            {
                LoadAllSavedData(pathMode);

                //Verify that all type are well added to the dictionnary
                foreach (string i in classToLoad)
                {
                    Type type = Type.GetType(i);

                    if (!savedDataDictionnary.ContainsKey(type))
                    {
                        SavedData s = Activator.CreateInstance(type) as SavedData;
                        AddSavedDataToDictionnary(s,-1);

                        s.dataVersion = dataVersion;
                        s.timestamp = 0;
                        s.OnDataCreated(dataVersion);

                        if (OnDataLoaded != null)
                            OnDataLoaded(this, s);
                    }
                }
            }
            else
            {
                foreach (string i in classToLoad)
                {
                    try
                    {
                        Type type = Type.GetType(i);
                        LoadSavedData(type, pathMode);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// Load all the saved data in the persistentDataSystem directory.
        /// </summary>
        public List<SavedData> LoadAllSavedData(PathMode pathMode = PathMode.PLAYER)
        {
            List<SavedData> savedDataList = new List<SavedData>();

            //The directory does not exist, return immediatly
            if ((saveMode == SaveMode.SINGLE_FILE && !Directory.Exists(pathMode ==  PathMode.DEFAULT ? singleDefaultFileDirectoryPath : singlePlayerFileDirectoryPath))
                || (saveMode == SaveMode.MULTIPLE_FILE && !Directory.Exists(pathMode == PathMode.DEFAULT ? multipleDefaultFilesDirectoryPath : multiplePlayerFilesDirectoryPath)))
            {
                return savedDataList;
            }

            savedDataDictionnary.Clear();

            if (saveMode == SaveMode.MULTIPLE_FILE)
            {
                string[] directories;

                if (pathMode == PathMode.PLAYER)
                    directories = Directory.GetDirectories(multiplePlayerFilesDirectoryPath);
                else
                    directories = Directory.GetDirectories(multipleDefaultFilesDirectoryPath);

                for (int i = 0; i < directories.Length; i++)
                {
                    string[] files = Directory.GetFiles(directories[i], "*", SearchOption.AllDirectories);

                    for (int j = 0; j < files.Length; j++)
                    {
                        SavedData s = LoadSavedData(files[j], j, pathMode == PathMode.DEFAULT);

                        if (s != null)
                        {
                            savedDataList.Add(s);

                            if (OnDataLoaded != null)
                            {
                                OnDataLoaded(this, s);
                            }
                        }
                    }
                }
            }
            else
            {
                try
                {
                    string dataPath;

                    if (pathMode == PathMode.PLAYER)
                        dataPath = singlePlayerFileDirectoryPath + SingleFileName + AutomaticSerializationFileExtension;
                    else
                        dataPath = singleDefaultFileDirectoryPath + SingleFileName + AutomaticSerializationFileExtension;

                    BinaryFormatter bf = new BinaryFormatter();
                    using (FileStream fs = File.Open(dataPath, FileMode.Open, FileAccess.Read))
                    {
                        savedDataDictionnary = (Dictionary<Type, List<SavedData>>)bf.Deserialize(fs);
                        fs.Close();
                    }

                    //If version is different, reinit with default file
                    for (int i = 0; i < savedDataList.Count; i++)
                    {
                        if (!CheckDataversion(dataVersion, savedDataList[i]))
                        {
                            if (pathMode == PathMode.PLAYER)
                            {
                                return LoadAllSavedData(PathMode.DEFAULT);
                            }
                            else
                                return new List<SavedData>(); //Considere Data are all outdated
                        }

                        savedDataList.Add(savedDataList[i]);
                    }
                }
                catch
                {
#if DEBUG || UNITY_EDITOR
                 Debug.LogWarning("Unable to load persistent data");
#endif
                }
            }

            return savedDataList;
        }

        /// <summary>
        /// Load saved data of the specified type, if no such file exist, one saved data will be Init.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>null is used with single file system<</returns>
        SavedData LoadSavedData(Type type, PathMode pathMode = PathMode.PLAYER)
        {
            if (saveMode == SaveMode.SINGLE_FILE)
                return null;

            SavedData savedData = null;
            ClearSavedDataList(type);

            try
            {
                string dataPath;

                if (pathMode == PathMode.PLAYER)
                    dataPath = System.IO.Path.Combine(System.IO.Path.Combine(multiplePlayerFilesDirectoryPath, type.Name), "0");
                else
                    dataPath = System.IO.Path.Combine(System.IO.Path.Combine(multipleDefaultFilesDirectoryPath, type.Name), "0");

                if (type.GetInterface(typeof(IFullSerializationControl).Name) != null)
                    dataPath += ControlledSerializationFileExtension;
                else
                    dataPath += AutomaticSerializationFileExtension;


                savedData = LoadSavedData(dataPath, 0, pathMode == PathMode.DEFAULT);

                if (savedData == null)
                {
                    throw new System.ArgumentException("No data of this type or incompatible version");
                }
                else if (pathMode == PathMode.DEFAULT)
                    savedData.OnDefaultDataLoaded();
            }
            catch
            {
#if DEBUG || UNITY_EDITOR
                Debug.Log("Load save at path: " + pathMode);
#endif

                if (pathMode == PathMode.PLAYER)
                    return LoadSavedData(type, PathMode.DEFAULT);

#if DEBUG || UNITY_EDITOR
                Debug.Log("New instance of " + type + " loaded");
#endif

                savedData = Activator.CreateInstance(type) as SavedData;
                savedData.dataVersion = dataVersion;
                savedData.timestamp = 0;
                savedData.OnDataCreated(dataVersion);

                AddSavedDataToDictionnary(savedData, -1);

                if (OnDataLoaded != null)
                    OnDataLoaded(this, savedData);
            }

            return savedData;
        }

        /// <summary>
        /// Load the first saved data of type T, if no such file exist, saved data will be Init.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>null if used with single file system</returns>
        public T LoadSavedData<T>(PathMode pathMode = PathMode.PLAYER) where T : SavedData
        {
            return (T)LoadSavedData(typeof(T), pathMode);
        }


        /// <summary>
        /// Load all saved data of the type. If no savedData of this type exist, this will NOT create a new one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="maxNumber"></param>
        /// <returns></returns>
        public List<T> LoadAllSavedData<T>(int maxNumber = int.MaxValue, PathMode pathMode = PathMode.PLAYER) where T : SavedData
        {
            if (saveMode == SaveMode.SINGLE_FILE)
                return null;

            List<T> savedDataList = new List<T>();
            SavedData savedData = null;
            Type type = typeof(T);
            ClearSavedDataList(type);

            for (int i = 0; i < maxNumber; i++)
            {
                try
                {
                    string playerDataPath;
                    string defaultDataPath;

                    playerDataPath = System.IO.Path.Combine(System.IO.Path.Combine(multiplePlayerFilesDirectoryPath, type.Name), i.ToString());
                    defaultDataPath = System.IO.Path.Combine(System.IO.Path.Combine(multipleDefaultFilesDirectoryPath, type.Name), i.ToString());

                    if (type.GetInterface(typeof(IFullSerializationControl).Name) != null)
                    {
                        playerDataPath += ControlledSerializationFileExtension;
                        defaultDataPath += ControlledSerializationFileExtension;
                    }
                    else
                    {
                        playerDataPath += AutomaticSerializationFileExtension;
                        defaultDataPath += AutomaticSerializationFileExtension;
                    }

                    if (pathMode == PathMode.PLAYER)
                        savedData = LoadSavedData(playerDataPath, i, false);
                    else
                        savedData = LoadSavedData(defaultDataPath, i, true);

                    //If player data are not present or version is not the same, try with default
                    if (savedData == null && pathMode == PathMode.PLAYER)
                    {
                        savedData = LoadSavedData(defaultDataPath, i, false);
                    }

                    if (savedData != null)
                        savedDataList.Add((T)savedData);
                    else
                        break;
                }
                catch
                {
                    break;
                }
            }

            return savedDataList;
        }



        /// <summary>
        /// Return a T type, find in data dictionnary or create
        /// This function will try to load the data if she is not already loaded
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetSavedData<T>(PathMode pathMode = PathMode.PLAYER) where T : SavedData, new()
        {
            List<SavedData> savedDataList;
            savedDataDictionnary.TryGetValue(typeof(T), out savedDataList);

            if (savedDataList != null && savedDataList.Count > 0)
            {
                object send = savedDataList[0];
                send = System.Convert.ChangeType(send, typeof(T));

                return (T)send;
            }
            //This data is not in the dictionnary
            else
            {
                //It's not in the dictionnary, so if use multiple file LoadIt with the file
                if (saveMode == SaveMode.MULTIPLE_FILE)
                {
                    return LoadSavedData<T>(pathMode);
                }
                //Create a new instance of T
                else
                {
                    T newSavedData = new T();
                    AddSavedDataToDictionnary(newSavedData,-1);

                    return newSavedData;
                }
            }
        }

        /// <summary>
        /// Return a T type, find in data dictionnary or create
        /// This function will try to load the data if she is not already loaded
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public SavedData GetSavedData(Type t, PathMode pathMode = PathMode.PLAYER)
        {
            List<SavedData> savedDataList;
            savedDataDictionnary.TryGetValue(t, out savedDataList);

            if (savedDataList != null && savedDataList.Count > 0)
            {
                SavedData send = savedDataList[0];

                return send;
            }
            //This data is not in the dictionnary
            else
            {
                //It's not in the dictionnary, so if use multiple file LoadIt with the file
                if (saveMode == SaveMode.MULTIPLE_FILE)
                {
                    return LoadSavedData(t,pathMode);
                }
                //Create a new instance of T
                else
                {
                    SavedData newSavedData = Activator.CreateInstance(t) as SavedData;
                    AddSavedDataToDictionnary(newSavedData, 0);

                    return newSavedData;
                }
            }
        }


        /// <summary>
        /// Return a T type in the persistent data dictionnary.
        /// If T type is not present in the dictionnary, this function will try to load saved files.
        /// Load saved file work only for multiple file system.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="maxNumber"></param>
        /// <returns></returns>
        public List<T> GetAllSavedData<T>(int maxNumber = int.MaxValue, PathMode pathMode = PathMode.PLAYER) where T : SavedData, new()
        {
            List<SavedData> savedDataList;
            savedDataDictionnary.TryGetValue(typeof(T), out savedDataList);

            if (savedDataList != null)
            {
                return savedDataList as List<T>;
            }
            //This data is not in the dictionnary
            else
            {
                //It's not in the dictionnary, so if use multiple file LoadIt with the file
                if (saveMode == SaveMode.MULTIPLE_FILE)
                {
                    return LoadAllSavedData<T>(maxNumber, pathMode);
                }
            }

            return new List<T>();
        }


        /// <summary>
        /// Save all data in the PersistentDataSystem dictionnary
        /// </summary>
        public void SaveAllData(PathMode pathMode = PathMode.PLAYER)
        {
            foreach (List<SavedData> sdList in savedDataDictionnary.Values)
            {
                foreach (SavedData sd in sdList)
                {
                    if (OnBeginDataSaving != null)
                        OnBeginDataSaving(this, sd);
                }
            }

            if (saveMode == SaveMode.SINGLE_FILE)
            {
                string dataPath;

                if (pathMode == PathMode.PLAYER)
                    dataPath = singlePlayerFileDirectoryPath;
                else
                    dataPath = singleDefaultFileDirectoryPath;

                if (!Directory.Exists(dataPath))
                    Directory.CreateDirectory(dataPath);

                dataPath += SingleFileName + AutomaticSerializationFileExtension;

                BinaryFormatter bf = new BinaryFormatter();
                using (FileStream fs = File.Create(dataPath))
                {
                    bf.Serialize(fs, savedDataDictionnary);
                    fs.Close();
                }

                foreach (List<SavedData> sdList in savedDataDictionnary.Values)
                {
                    foreach (SavedData sd in sdList)
                    {
                        sd.filePath = dataPath;
                        sd.fileNumber = 0;
                        OnDataSaved?.Invoke(this, sd);
                    }
                }
            }
            else
            {
                foreach (List<SavedData> sdList in savedDataDictionnary.Values)
                {
                    foreach (SavedData sd in sdList)
                    {
                        SaveData(sd, pathMode);
                    }
                }
            }
        }

        /// <summary>
        /// Only save the first instance of the specify class
        /// doesn't work if not multiple file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void SaveData<T>(PathMode loadMode = PathMode.PLAYER) where T : SavedData, new()
        {
            if (saveMode == SaveMode.SINGLE_FILE)
            {
                Debug.LogError("You don't use multiple file, you can't save only one class. Use SaveData() instead");
                return;
            }

            //The class is in persistent data, find it and save it
            T savedData = GetSavedData<T>();
            SaveData(savedData, loadMode);
        }


        /// <summary>
        /// Will save your data only if it present on persistentData
        /// </summary>
        /// <param name="savedData"></param>
        public void SaveData(SavedData savedData, PathMode loadMode = PathMode.PLAYER)
        {
            if (saveMode == SaveMode.SINGLE_FILE)
            {
                Debug.LogError("You don't use multiple file, you can't save only one saved data. Use SaveData() instead");
                return;
            }

            string dataPath = savedData.filePath;
            Type type = savedData.GetType();

            List<SavedData> savedDataList;
            savedDataDictionnary.TryGetValue(savedData.GetType(), out savedDataList);

            if (savedDataList == null)
            {
                Debug.LogError("There is no type of this data in the dictionnary, this is not possible. Use AddNewSavedData");
                return;
            }

            if (!savedDataList.Contains(savedData))
            {
                Debug.LogError("This data is not in the dictionnary, this is not possible. Use AddNewSavedData");
                return;
            }

            int cpt = 0;
            foreach (SavedData sd in savedDataList)
            {
                if (OnBeginDataSaving != null)
                    OnBeginDataSaving(this, sd);

                if (sd == savedData)
                {
                    if (loadMode == PathMode.PLAYER)
                        dataPath = System.IO.Path.Combine(multiplePlayerFilesDirectoryPath, type.Name);
                    else
                        dataPath = System.IO.Path.Combine(multipleDefaultFilesDirectoryPath, type.Name);

                    if (!Directory.Exists(dataPath))
                        Directory.CreateDirectory(dataPath);

                    dataPath += "/" + cpt;

                    if (savedData is IFullSerializationControl)
                    {
                        dataPath += ControlledSerializationFileExtension;
                        using (FileStream fs = File.Create(dataPath))
                        {
                            BinaryWriter writer = new BinaryWriter(fs);
                            writer.Write(type.Name);
                            writer.Write(sd.dataVersion);
                            writer.Write(new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds());

                            ((IFullSerializationControl)savedData).WriteObjectData(writer);
                            fs.Close();                       
                        }
                    }
                    else
                    {
                        dataPath += AutomaticSerializationFileExtension;
                        BinaryFormatter bf = new BinaryFormatter();
                        using (FileStream fs = File.Create(dataPath))
                        {
                            bf.Serialize(fs, savedData);
                            fs.Close();
                        }
                    }

                    savedData.filePath = dataPath;
                    savedData.fileNumber = cpt;

                    OnDataSaved?.Invoke(this, sd);
                }

                cpt++;
            }
        }


        /// <summary>
        /// Contains the "."
        /// </summary>
        /// <param name="savedData"></param>
        /// <returns></returns>
        public string GetFileExtension(SavedData savedData)
        {
            if (savedData is IFullSerializationControl)
            {
                return ControlledSerializationFileExtension;
            }
            else
                return AutomaticSerializationFileExtension;
        }

        /// <summary>
        /// This will add a type T to be saved on the next SaveData call.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AddNewSavedData<T>() where T : SavedData, new()
        {
            T newSavedData = new T();
            newSavedData.dataVersion = dataVersion;
            newSavedData.timestamp = 0;
            AddSavedDataToDictionnary(newSavedData, -1);
            newSavedData.OnDataCreated(dataVersion);

            return newSavedData;
        }

        public void AddSavedDataToDictionnary(SavedData savedData, int fileNumber)
        {
            Type type = savedData.GetType();
            List<SavedData> dataList;
            savedDataDictionnary.TryGetValue(type, out dataList);

            if (dataList == null)
            {
                dataList = new List<SavedData>();
                savedDataDictionnary.Add(type, dataList);
            }

            if(fileNumber == -1)
            {
                fileNumber = dataList.Count;
            }

            savedData.fileNumber = fileNumber;
            savedData.dataVersion = this.dataVersion;

            for (int i = 0; i < dataList.Count; i++)
            {
                if(dataList[i].fileNumber == fileNumber)
                {
                    dataList[i] = savedData;
                    Debug.Log("Replacing: " + savedData + " in dictionary");
                    return;
                }
            }

            dataList.Add(savedData);

            dataList.OrderBy((x) => x.fileNumber);
        }

        private void ClearSavedDataList(Type type)
        {
            List<SavedData> dataList;
            savedDataDictionnary.TryGetValue(type, out dataList);

            if (dataList != null)
            {
                dataList.Clear();
            }
        }

        /// <summary>
        /// Load a savedData with a filePath
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileNumber"></param>
        /// <returns>null if the data is not in the good version or does not exist</returns>
        private SavedData LoadSavedData(string filePath, int fileNumber, bool isInStreamingAsset)
        {
            SavedData savedData = RetrieveSavedData(filePath, filePath, isInStreamingAsset);

            if (savedData != null)
            {
                AddSavedDataToDictionnary(savedData, fileNumber);

                if (OnDataLoaded != null)
                    OnDataLoaded(this, savedData);
            }

            return savedData;
        }


        public Type GetDataType(string filePath, string fileName)
        {
            SavedData savedData = null;

            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                //Android is using .jar file, require another access !
                if (isInStreamingAsset)
                {
                     Debug.Log("Loading Default Data");
                    WWW wwwReader = new WWW(filePath);
                    while (!wwwReader.isDone) { };

                    if (fileName.EndsWith(AutomaticSerializationFileExtension))
                    {
                        BinaryFormatter bf = new BinaryFormatter();

                        using (MemoryStream ms = new MemoryStream(wwwReader.bytes))
                        {
                            savedData = bf.Deserialize(ms) as SavedData;
                            ms.Close();
                            return savedData.GetType();
                        }
                    }
                    else if (fileName.EndsWith(ControlledSerializationFileExtension))
                    {
                        using (MemoryStream ms = new MemoryStream(wwwReader.bytes))
                        {
                            BinaryReader reader = new BinaryReader(ms);
                            ms.Close();
                            return Type.GetType(reader.ReadString());
                        }
                    }
                }
                else
                {
#endif
                if (fileName.EndsWith(AutomaticSerializationFileExtension))
                {
                    BinaryFormatter bf = new BinaryFormatter();

                    using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
                    {
                        savedData = bf.Deserialize(fs) as SavedData;
                        fs.Close();
                        return savedData.GetType();
                    }
                }
                else if (fileName.EndsWith(ControlledSerializationFileExtension))
                {
                    using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
                    {
                        BinaryReader reader = new BinaryReader(fs);
                        return Type.GetType(reader.ReadString());
                    }
                }
#if UNITY_ANDROID && !UNITY_EDITOR
                }
#endif
                Debug.Log(fileName + " ----> Type not found: " + filePath);
                return null;
            }
            catch (System.Exception e)
            {
                Debug.Log(fileName + " ----> Type not found: " + e);
                return null;
            }
        }

    public SavedData RetrieveSavedData(string filePath,string fileName, bool isInStreamingAsset)
    {
        SavedData savedData = null;

        try
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                //Android is using .jar file, require another access !
                if (isInStreamingAsset)
                {
                     Debug.Log("Loading Default Data");
                    WWW wwwReader = new WWW(filePath);
                    while (!wwwReader.isDone) { };

                    if (fileName.EndsWith(AutomaticSerializationFileExtension))
                    {
                        BinaryFormatter bf = new BinaryFormatter();

                        using (MemoryStream ms = new MemoryStream(wwwReader.bytes))
                        {
                            savedData = bf.Deserialize(ms) as SavedData;
                            ms.Close();
                        }
                    }
                    else if (fileName.EndsWith(ControlledSerializationFileExtension))
                    {
                        using (MemoryStream ms = new MemoryStream(wwwReader.bytes))
                        {
                            BinaryReader reader = new BinaryReader(ms);
                            Type type = Type.GetType(reader.ReadString());
                            savedData = Activator.CreateInstance(type) as SavedData;
                            savedData.dataVersion = reader.ReadInt32();
                            savedData.timestamp = reader.ReadInt64();

                            ((IFullSerializationControl)savedData).ReadObjectData(reader);
                            savedData.dataVersion = this.dataVersion;
                            ms.Close();
                        }
                    }
                }
                else
                {
#endif
                if (fileName.EndsWith(AutomaticSerializationFileExtension))
            {
                BinaryFormatter bf = new BinaryFormatter();

                using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    savedData = bf.Deserialize(fs) as SavedData;
                    fs.Close();
                }
            }
            else if (fileName.EndsWith(ControlledSerializationFileExtension))
            {
                using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    BinaryReader reader = new BinaryReader(fs);
                    Type type = Type.GetType(reader.ReadString());
                    savedData = Activator.CreateInstance(type) as SavedData;
                    savedData.dataVersion = reader.ReadInt32();
                    savedData.timestamp = reader.ReadInt64();

                    ((IFullSerializationControl)savedData).ReadObjectData(reader);
                    fs.Close();
                }
            }
#if UNITY_ANDROID && !UNITY_EDITOR
                }
#endif
                if (savedData != null) //.meta for Unity can be here in editor T-T
                {
                    savedData.filePath = filePath;
                    savedData.dataVersion = this.dataVersion;
                }

            return savedData;
        }
        catch(System.Exception e)
        {
                Debug.Log(fileName + " ----> Retrieve save file failed: " + e);
            return null;
        }
    }


        bool CheckDataversion(int version, SavedData savedData)
        {
            if (savedData.dataVersion != version)
            {
                //The version is not the same after all importer, so this data version is not compatible with the current persistentDataSystem version
                if (savedData.dataVersion != version)
                {

#if DEBUG || UNITY_EDITOR
                    Debug.LogWarning("New version loaded for : " + savedData.GetType());
#endif
                    //Create new SavedData
                    savedData = Activator.CreateInstance(savedData.GetType()) as SavedData;
                    savedData.dataVersion = dataVersion;
                    savedData.timestamp = 0;
                    savedData.OnDataCreated(dataVersion);

                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// This will erase all saved files
        /// </summary>
        public void EraseAllSavedData(PathMode pathMode = PathMode.PLAYER)
        {
            if (pathMode == PathMode.PLAYER)
            {
                if (Directory.Exists(automaticPlayerSavedDataDirectoryPath))
                    Directory.Delete(automaticPlayerSavedDataDirectoryPath, true);
            }
            else
            {
                if (Directory.Exists(Application.streamingAssetsPath + "/" + PersistentDataSystemDirectory))
                    Directory.Delete(Application.streamingAssetsPath + "/" + PersistentDataSystemDirectory, true);
            }

            savedDataDictionnary.Clear();
        }

        /// <summary>
        /// Erase all data of type T saved in the provided path
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pathMode"></param>
        public void EraseAllSavedData<T>(PathMode pathMode = PathMode.PLAYER) where T : SavedData
        {
            Type type = typeof(T);

            if (pathMode == PathMode.PLAYER)
            {
                if (Directory.Exists(System.IO.Path.Combine(multiplePlayerFilesDirectoryPath, type.Name)))
                    Directory.Delete(System.IO.Path.Combine(multiplePlayerFilesDirectoryPath, type.Name), true);

                if (Directory.Exists(System.IO.Path.Combine(singlePlayerFileDirectoryPath, type.Name)))
                    Directory.Delete(System.IO.Path.Combine(singlePlayerFileDirectoryPath, type.Name), true);
            }
            else
            {
                if (Directory.Exists(System.IO.Path.Combine(multipleDefaultFilesDirectoryPath, type.Name)))
                    Directory.Delete(System.IO.Path.Combine(multipleDefaultFilesDirectoryPath, type.Name), true);

                if (Directory.Exists(System.IO.Path.Combine(singleDefaultFileDirectoryPath, type.Name)))
                    Directory.Delete(System.IO.Path.Combine(singleDefaultFileDirectoryPath, type.Name), true);
            }

            savedDataDictionnary.Remove(type);
        }

        public void ReplaceAtIndex(SavedData Data, int index)
        {

        }

        /// <summary>
        /// Unload saved data of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void UnloadSavedData<T>() where T : SavedData
        {
            ClearSavedDataList(typeof(T));
        }

        /// <summary>
        /// Unload saved data by clearing dictionnary
        /// </summary>
        public void UnloadAllSavedData()
        {
            savedDataDictionnary.Clear();
        }

        public void OnApplicationPause(bool paused)
        {

            if (!autoSave)
                return;

            if (paused && savedDataDictionnary != null)
            {
                SaveAllData();

#if UNITY_EDITOR || DEBUG
                Debug.LogWarning("Data Save On Pause By AUTO_SAVE_MANAGEMENT");
#endif
            }
#if UNITY_EDITOR || DEBUG
            else
                Debug.LogWarning("Data Not Save On Pause By AUTO_SAVE_MANAGEMENT");
#endif
        }

        public void OnApplicationQuit()
        {
            if (!autoSave)
                return;

            if (savedDataDictionnary != null)
            {
                SaveAllData();

#if UNITY_EDITOR || DEBUG
                Debug.LogWarning("Data Saved On Application Quit By AUTO_SAVE_MANAGEMENT");
#endif
            }
            else
            {
#if UNITY_EDITOR || DEBUG
                Debug.LogWarning("Data Not Saved On Application Quit due to null data By AUTO_SAVE_MANAGEMENT");
#endif
            }
        }
    }
}