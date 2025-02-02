// This script is optional, only for the demo scene

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace KevinIglesias
{
    [ExecuteInEditMode]
    public class BasicMotionsRPLoaderFREE : MonoBehaviour
    {
        [SerializeField]
        public List<BasicMotionsMaterialTypeFREE> materialTypes;

        void OnValidate()
        {
            if(materialTypes == null)
            {
                return;
            }
            
            for(int i = 0; i < materialTypes.Count; i++)
            {
                if(materialTypes[i].defaultMaterial != null)
                {
                    materialTypes[i].materialName = materialTypes[i].defaultMaterial.name;
                }
                
                for(int j = 0; j < materialTypes[i].meshes.Length; j++)
                {
                    if(materialTypes[i].meshes[j] != null)
                    {
                        Material materialToApply = materialTypes[i].defaultMaterial;
                        if(GraphicsSettings.currentRenderPipeline != null)
                        {
                            if(GraphicsSettings.currentRenderPipeline.GetType().ToString().Contains("HighDefinition"))
                            {
                                materialToApply = materialTypes[i].hDRPMaterial;
                            }else{
                                materialToApply = materialTypes[i].uRPMaterial;
                            }
                        }
                        
                        SkinnedMeshRenderer sMR = materialTypes[i].meshes[j].GetComponent<SkinnedMeshRenderer>();
                        if(sMR)
                        {
                            materialTypes[i].meshes[j].GetComponent<SkinnedMeshRenderer>().material = materialToApply;
                        }else{
                            materialTypes[i].meshes[j].GetComponent<MeshRenderer>().material = materialToApply;
                        }
                    }
                }
            }
        }
        
        void OnEnable()
        {
            OnValidate();
        }
        
        void Update(){}
    }
    
    [System.Serializable]
    public class BasicMotionsMaterialTypeFREE
    {
        [HideInInspector]
        public string materialName;
        
        public Material defaultMaterial;
        public Material uRPMaterial;
        public Material hDRPMaterial;
        
        public GameObject[] meshes;
    }
}
#endif

