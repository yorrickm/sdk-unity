using UnityEngine;
using System.Collections;

public static class RoarUIUtil
{
	public static Rect UnionRect(Rect[] rects)
	{
		if (rects == null)
			return new Rect(0,0,0,0);
		
		float x = rects[0].x;
		float y = rects[0].y;
		float w = rects[0].width;
		float h = rects[0].height;
		
		for (int i=1; i<rects.Length; i++)
		{
			Rect rect = rects[i];
			x = Mathf.Min(x, rect.x);
			y = Mathf.Min(y, rect.y);
			w = Mathf.Max(w, (rect.x - x) + rect.width);
			h = Mathf.Max(h, (rect.y - y) + rect.height);
		}
		
		return new Rect(x,y,w,h);
	}
}
