using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public LoadingBehaviour loadingUi;


    // StartCoroutine(LoadNewScene());

    // The coroutine runs on its own at the same time as Update() and takes an integer indicating which scene to load.
    public IEnumerator LoadNewScene(int scene)
    {
        this.loadingUi.gameObject.SetActive(true);
        // This line waits for 3 seconds before executing the next line in the coroutine.
        // This line is only necessary for this demo. The scenes are so simple that they load too fast to read the "Loading..." text.


        SceneManager.LoadScene(1);

        yield return null;
        //// Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
        //AsyncOperation async = SceneManager.LoadSceneAsync(scene);

        //// While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
        //while (!async.isDone)
        //{
        //    Debug.Log(async.progress);
        //    this.loadingUi.slider.value = (int)async.progress * 100;
        //    yield return null;
        //}

    }
}
