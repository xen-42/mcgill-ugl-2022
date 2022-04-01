using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
//using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;

public class PostProcessingController : MonoBehaviour
{
    private Volume volume;
  //  private Bloom bloom;
    private ChromaticAberration c_aberration;
    private LensDistortion lens_distortion;
    private Vignette vignette;
    private MotionBlur motion_blur;
      private ShadowsMidtonesHighlights smhLight;
      private Tonemapping tone;
    void Start()
    {
        volume = GetComponent<Volume>();
        
      //  volume.sharedProfile.TryGet<Bloom>(out bloom);
        volume.sharedProfile.TryGet<ChromaticAberration>(out c_aberration);
        volume.sharedProfile.TryGet<LensDistortion>(out lens_distortion);
        volume.sharedProfile.TryGet<Vignette>(out vignette);
        volume.sharedProfile.TryGet<MotionBlur>(out motion_blur);
        volume.sharedProfile.TryGet<ShadowsMidtonesHighlights>(out smhLight);

     //   bloom.intensity.value = 0f;
        c_aberration.intensity.value = 0f;
        lens_distortion.intensity.value = 0f;
        vignette.intensity.value = 0f;
        motion_blur.intensity.value = 0f;
        //smhLight.active = true;
        
        smhLight.midtones = new UnityEngine.Rendering.Vector4Parameter(new Vector4(0,0,0,0));
        Vector4 newV4 = new Vector4(0f, 1f, 1f, 0f);
      smhLight.midtones = new UnityEngine.Rendering.Vector4Parameter(newV4);
       smhLight.shadows = new UnityEngine.Rendering.Vector4Parameter(newV4);
    }

    public void EnableAllOverrides(){
     //bloom.active = true;
        c_aberration.active = true;
        lens_distortion.active = true;
        vignette.active = true;
        motion_blur.active = true;
    }

    public void DisableAllOverrides(){
      //  bloom.active = false;
        c_aberration.active = false;
        lens_distortion.active = false;
        vignette.active = false;
        motion_blur.active = false;
    }

    public void UpdateStressVision(float stress){
      //  bloom.intensity.value = stress * 0.4f; 
        c_aberration.intensity.value = stress * 0.02f;
        lens_distortion.intensity.value = stress * 0.008f;
        vignette.intensity.value = stress * 0.01f;
        motion_blur.intensity.value = stress * 0.02f;
    }

    public void UpdateSMH (int timelimit){
      
    }

}


