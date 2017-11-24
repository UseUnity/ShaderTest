﻿using UnityEngine;

/// <summary>
/// 运动模糊（积累模糊）
/// </summary>
public class MotionBlur : PostEffectsBase
{

    [Range(0.0f, 0.9f)]                         // 为1的时候完全代替当前帧的渲染结果
    public float blurAmount = 0.5f;             // 模糊参数

    private RenderTexture accumulationTexture;  // 保存之前图像的叠加效果

    void OnDisable()
    {
        DestroyImmediate(accumulationTexture);  // 用完就销毁，下一次开始应用这个重新叠加
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (TargetMaterial != null)
        {
            // 创建积累图像
            if (accumulationTexture == null || accumulationTexture.width != src.width || accumulationTexture.height != src.height)
            {
                DestroyImmediate(accumulationTexture);
                accumulationTexture = new RenderTexture(src.width, src.height, 0);
                accumulationTexture.hideFlags = HideFlags.HideAndDontSave;          // 变量不显示在Hierarchy中，也不会保存到场景
                Graphics.Blit(src, accumulationTexture);        // 原始图像存入积累纹理
            }

            // 表明需要进行一个恢复操作。渲染恢复操作：发生在渲染到纹理，而该纹理有没有被提前情况或销毁情况下。
            accumulationTexture.MarkRestoreExpected();          // accumulationTexture就不需要提前清空了

            TargetMaterial.SetFloat("_BlurAmount", 1.0f - blurAmount);

            // 混合当前屏幕和之前存的混合图像
            Graphics.Blit(src, accumulationTexture, TargetMaterial);
            // 最后输出混合图像
            Graphics.Blit(accumulationTexture, dest);
        }
        else
            Graphics.Blit(src, dest);
    }
}
