using BepInEx;
using HarmonyLib;
using System.Reflection;
using LC_API;
using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System;
using System.Runtime.CompilerServices;
using UnityEngine.Animations;

namespace GiantChibi
{
	public class ChibiModel : MonoBehaviour
	{
		private ForestGiantAI controller;

		public void Start()
		{
			Debug.Log("[MonAmiral] Replacing the Forest Giant model.");

			// Fetch the base model.
			this.controller = this.GetComponentInChildren<ForestGiantAI>();
			this.GetComponentInChildren<LODGroup>().enabled = false;
			SkinnedMeshRenderer[] baseRenderers = this.GetComponentsInChildren<SkinnedMeshRenderer>();

			GameObject modelInstance = null;
			try
			{
				// Fetch & prepare the new model.
				GameObject modelPrefab = LC_API.BundleAPI.BundleLoader.GetLoadedAsset<GameObject>("assets/GiantChibi/ChibiGiant.fbx");
				Texture texture = LC_API.BundleAPI.BundleLoader.GetLoadedAsset<Texture>("assets/GiantChibi/Chibmin_ALB.png");
				Material material = new Material(baseRenderers[0].material);
				material.SetTexture("_BaseColorMap", texture);
				material.SetTexture("_BumpMap", null);

				// Instantiate the model & assign the material.
				modelInstance = GameObject.Instantiate(modelPrefab, this.transform);
				modelInstance.transform.position = Vector3.zero;
				modelInstance.transform.rotation = Quaternion.identity;

				SkinnedMeshRenderer[] chibiRenderers = modelInstance.GetComponentsInChildren<SkinnedMeshRenderer>();
				foreach (SkinnedMeshRenderer renderer in chibiRenderers)
				{
					renderer.gameObject.layer = baseRenderers[0].gameObject.layer;
					renderer.material = material;
				}

				// Reassign the bones so that the animation perfectly matches.
				Transform chibiArmature = modelInstance.transform.Find("metarig");
				Transform giantArmature = this.transform.Find("FGiantModelContainer").Find("AnimContainer").Find("metarig");
				this.ParentBoneRecursively(chibiArmature, giantArmature);

				// Disable the base model only if everything went right.
				foreach (SkinnedMeshRenderer renderer in baseRenderers)
				{
					renderer.gameObject.SetActive(false);
				}
			}
			catch (System.Exception e)
			{
				Debug.LogException(e);

				if (modelInstance)
				{
					GameObject.Destroy(modelInstance);
					GameObject.Destroy(this);
				}
			}
		}

		public void Update()
		{
			// TODO: Voicelines & Mouth opening.
		}

		private void ParentBoneRecursively(Transform boneToParent, Transform newParent)
		{
			boneToParent.parent = newParent;
			boneToParent.localPosition = Vector3.zero;
			boneToParent.localRotation = Quaternion.identity;

			// Operate in reverse since we're removing children.
			for (int i = boneToParent.childCount - 1; i >= 0; i--)
			{
				Transform child = boneToParent.GetChild(i);
				Transform newChild = newParent.Find(child.name);
				if (newChild)
				{
					this.ParentBoneRecursively(child, newChild);
				}
			}
		}
	}
}