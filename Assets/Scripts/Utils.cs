using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils {
	public static void CopyTransformsRecurse (Transform src, Transform dst) {
		dst.position = src.position;
		dst.rotation = src.rotation;
		dst.localScale = src.localScale;
		dst.gameObject.SetActive(src.gameObject.activeSelf);
		
		for(int i = 0; i < dst.childCount; i++) {
			Transform child = dst.GetChild(i);
			Transform curSrc = src.Find(child.name);
			
			if (curSrc) CopyTransformsRecurse(curSrc, child);
		}
	}
}
