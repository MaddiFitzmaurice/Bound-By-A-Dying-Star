#ifndef ADDITIONAL_LIGHT_INCLUDED
#define ADDITIONAL_LIGHT_INCLUDED

// For the main light in the scene
void MainLight_float(float3 WorldPos, 
    out float3 Direction, out float3 Color, out float Attenuation)
{
// If viewing in Shadergraph window, fake some light
#ifdef SHADERGRAPH_PREVIEW
    Direction = normalize(float3(1.0f, 1.0f, 0.0f));
    Color = 1.0f;
    Attenuation = 1.0f;
    // If viewing in scene/game, get the main light
#else
    Light mainLight = GetMainLight();
    Direction = mainLight.direction;
    Color = mainLight.color;
    Attenuation = mainLight.distanceAttenuation;
#endif
    
}

// For additional lights in scene
void AdditionalLight_float(float3 WorldPos, int lightID, 
    out float3 Direction, out float3 Color, out float Attenuation)
{
    // Fake light
    Direction = normalize(float3(1.0f, 1.0f, 0.0f));
    Color = 0.0f;
    Attenuation = 0.0f;
    
    // If not using shadergraph view
#ifndef SHADERGRAPH_PREVIEW
    int lightCount = GetAdditionalLightsCount();
    
    if (lightID < lightCount)
    {
        Light light = GetAdditionalLight(lightID, WorldPos);
        Direction = light.direction;
        Color = light.color;
        Attenuation = light.distanceAttenuation;
    }
#endif        
}

void AllAdditionalLights_float(float3 WorldPos, float3 WorldNormal, 
    float2 CutoffThresholds, out float3 LightColor)
{
    LightColor = 0.0f;
    
#ifndef SHADERGRAPH_PREVIEW
    int lightCount = GetAdditionalLightsCount();
    
    for (int i = 0; i < lightCount; i++)
    {
        Light light = GetAdditionalLight(i, WorldPos);
        
        float3 color = dot(light.direction, WorldNormal);
        color = smoothstep(CutoffThresholds.x, CutoffThresholds.y, color);
        color *= light.color;
        color *= light.distanceAttenuation;
        
        LightColor += color;
    }
#endif
}
#endif