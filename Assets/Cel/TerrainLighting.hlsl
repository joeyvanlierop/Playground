#ifndef LIGHTING_CEL_SHADED_INCLUDED
#define LIGHTING_CEL_SHADED_INCLUDED

#ifndef SHADERGRAPH_PREVIEW
struct EdgeConstants {

   float highlightThreshold;
   float shadowThreshold;
   float distanceAttenuation;
   float shadowAttenuation;

};

struct ColorConstants {

   float3 highlight;
   float3 midtone;
   float3 shadow;

};

struct SurfaceVariables {

   float smoothness;
   float shininess;
   
   float3 normal;
   float3 view;

   EdgeConstants ec;

   ColorConstants cc;

};

float3 CalculateCelShading(Light l, SurfaceVariables s) {
   float attenuation = 
      smoothstep(0.0f, s.ec.distanceAttenuation, l.distanceAttenuation) * 
      smoothstep(0.0f, s.ec.shadowAttenuation, l.shadowAttenuation);

   float diffuse = saturate(dot(s.normal, l.direction));
   diffuse *= attenuation;

   float3 tone = s.cc.midtone;
   if(diffuse > s.ec.highlightThreshold)
   {
      tone = s.cc.highlight;
   }
   if(diffuse < s.ec.shadowThreshold)
   {
      tone = s.cc.shadow;
   }
   
   return l.color * tone;
}
#endif

void LightingCelShaded_float(float Smoothness, 
      float3 Position, float3 Normal, float3 View, float HighlightThreshold,
      float ShadowThreshold, float EdgeDistanceAttenuation,
      float EdgeShadowAttenuation, float3 Highlight,
      float3 Midtone, float3 Shadow, out float3 Color) {

#if defined(SHADERGRAPH_PREVIEW)
   Color = half3(0.5f, 0.5f, 0.5f);
#else
   SurfaceVariables s;
   s.smoothness = Smoothness;
   s.shininess = exp2(10 * Smoothness + 1);
   s.normal = normalize(Normal);
   s.view = SafeNormalize(View);
   s.ec.highlightThreshold = HighlightThreshold;
   s.ec.shadowThreshold = ShadowThreshold;
   s.ec.distanceAttenuation = EdgeDistanceAttenuation;
   s.ec.shadowAttenuation = EdgeShadowAttenuation;
   s.cc.highlight = Highlight;
   s.cc.midtone = Midtone;
   s.cc.shadow = Shadow;

#if SHADOWS_SCREEN
   float4 clipPos = TransformWorldToHClip(Position);
   float4 shadowCoord = ComputeScreenPos(clipPos);
#else
   float4 shadowCoord = TransformWorldToShadowCoord(Position);
#endif

   Light light = GetMainLight(shadowCoord);
   Color = CalculateCelShading(light, s);

   int pixelLightCount = GetAdditionalLightsCount();
   for (int i = 0; i < pixelLightCount; i++) {
      light = GetAdditionalLight(i, Position, 1);
      Color += CalculateCelShading(light, s);
   }
   
#endif
}

#endif