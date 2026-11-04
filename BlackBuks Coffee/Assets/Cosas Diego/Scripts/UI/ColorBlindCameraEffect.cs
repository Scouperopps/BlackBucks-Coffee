using UnityEngine;

[ExecuteInEditMode]
public class ColorblindCameraEffect : MonoBehaviour
{
    [SerializeField] private Material colorblindMaterial;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        int mode = PlayerPrefs.GetInt("ColorblindMode", 0);

        if (colorblindMaterial != null && mode > 0)
        {
            colorblindMaterial.SetInt("_Mode", mode);
            Graphics.Blit(source, destination, colorblindMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}