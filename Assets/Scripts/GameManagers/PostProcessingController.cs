using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
//using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;

public class PostProcessingController : MonoBehaviour
{
    private Volume volume;
    private Bloom bloom;
    private ChromaticAberration c_aberration;
    //private LensDistortion lens_distortion;
    private Vignette vignette;

    void Start()
    {
        volume = GetComponent<Volume>();
        
        volume.sharedProfile.TryGet<Bloom>(out bloom);
        volume.sharedProfile.TryGet<ChromaticAberration>(out c_aberration);
        //volume.sharedProfile.TryGet<LensDistortion>(out lens_distortion);
        volume.sharedProfile.TryGet<Vignette>(out vignette);
    }

    public void EnableAllOverrides(){
        bloom.active = true;
        c_aberration.active = true;
        //lens_distortion.active = true;
        vignette.active = true;
    }

    public void DisableAllOverrides(){
        bloom.active = false;
        c_aberration.active = false;
        //lens_distortion.active = false;
        vignette.active = false;
    }

}


