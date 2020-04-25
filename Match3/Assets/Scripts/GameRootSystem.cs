using BC_Solution;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRootSystem : MonoBehaviour
{
    [SerializeField]
    ScriptableScenePackage gameScenePackage;


    IEnumerator Start()
    {
        //There is a bug in Unity with loading scene at the very start
        //Wait 2 frames
        yield return null;
        yield return null;
        //

        yield return StartCoroutine(gameScenePackage.LoadScenePackageCoroutine(null, null,new string[]{this.gameObject.scene.name})); //Load the package containing the necessary for the game to run.

        SceneManager.UnloadSceneAsync(this.gameObject.scene.name); //Unload the gameRoot scene
    }


}
