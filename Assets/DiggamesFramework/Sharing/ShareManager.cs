using UnityEngine;
using System.Collections;

public class ShareManager : MonoBehaviour
{
    public Texture2D shareTexture;
    public string shareText = "Come try this awesome new game - Colors on the Brain!";

    public void FacebookPredefinedImageShare()
    {
        StartCoroutine(PostFBImage());
    }

    public void TwitterShare()
    {
#if UNITY_ANDROID
        AndroidSocialGate.StartShareIntent("Share Colors on the Brain!", shareText, shareTexture, "twi");
#elif UNITY_IOS
        IOSSocialManager.Instance.TwitterPost(shareText, null, shareTexture);
#endif
    }

    public void InstagramShare()
    {
#if UNITY_ANDROID
        AndroidSocialGate.StartShareIntent("Share Colors on the Brain!", shareText, shareTexture, "insta");
#elif UNITY_IOS
        IOSSocialManager.Instance.InstagramPost(shareTexture, shareText);
#endif
    }

    public void GooglePlusShare()
    {
#if UNITY_ANDROID
        AndroidSocialGate.StartGooglePlusShare(shareText, shareTexture);
#endif
    }

    private IEnumerator PostFBImage()
    {
        yield return new WaitForEndOfFrame();

#if UNITY_ANDROID
        AndroidSocialGate.StartShareIntent("Share Colors on the Brain!", shareText, shareTexture, "facebook.katana");
#elif UNITY_IOS
        IOSSocialManager.Instance.FacebookPost(shareText, null, shareTexture);
#endif
    }
}
