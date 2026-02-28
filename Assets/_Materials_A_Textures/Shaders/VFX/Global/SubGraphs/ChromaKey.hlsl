// Helper: Convertir RGB a YCrCb
// Y = Luminancia (Brillo), Cr/Cb = Crominancia (Color)
float3 RGBtoYCrCb(float3 c)
{
    float Y = 0.299 * c.r + 0.587 * c.g + 0.114 * c.b;
    float Cr = 0.713 * (c.r - Y);
    float Cb = 0.564 * (c.b - Y);
    return float3(Y, Cr, Cb);
}

// Nombre de la función: ChromaKey
void ChromaKey_float(float3 Input, float3 KeyColor, float Threshold, float Softness, float DespillAmount, out float3 OutRGB, out float OutAlpha)
{
    // 1. Conversión de espacio de color
    // Al convertir a YCrCb, aislamos el color de la iluminación.
    float3 inputYCrCb = RGBtoYCrCb(Input);
    float3 keyYCrCb = RGBtoYCrCb(KeyColor);

    // 2. Calcular distancia de Croma (Ignoramos Y)
    // Solo medimos la distancia en el plano de color (Cr, Cb). 
    // Esto hace que el filtro funcione aunque la pantalla verde tenga sombras fuertes.
    float chromaDist = distance(inputYCrCb.yz, keyYCrCb.yz);

    // 3. Calcular Alpha (Máscara)
    // Usamos smoothstep para un degradado suave entre transparente y opaco.
    float baseAlpha = smoothstep(Threshold, Threshold + Softness, chromaDist);

    // 4. Lógica de DESPILL (Eliminación de contaminación de color)
    // El "Spill" es el reflejo verde en la piel o ropa.
    // Calculamos un factor basado en qué tan cerca estamos del color clave.
    // (1.0 - baseAlpha) nos dice "qué tan verde es este pixel".
    float spillVal = (1.0 - baseAlpha) * DespillAmount;

    // Calculamos una versión en escala de grises (Luminancia) del pixel original.
    float3 lumaColor = float3(inputYCrCb.x, inputYCrCb.x, inputYCrCb.x);

    // Mezclamos el color original con su versión gris basada en el spill.
    // Esto neutraliza el borde verde volviéndolo gris oscuro/sombra, que se mezcla mejor.
    OutRGB = lerp(Input, lumaColor, spillVal);
    
    // Salida final del Alpha
    OutAlpha = baseAlpha;
}

// ---------------------------------------------------------
// VERSION HALF (Media calidad - Móviles/Optimización)
// Esta es la función que Unity no encontraba.
// ---------------------------------------------------------
void ChromaKey_half(half3 Input, half3 KeyColor, half Threshold, half Softness, half DespillAmount, out half3 OutRGB, out half OutAlpha)
{
    // Convertimos a float para el cálculo matemático (evita artefactos)
    float3 InputF = float3(Input);
    float3 KeyColorF = float3(KeyColor);
    
    // Reutilizamos la lógica llamando a la función float o repitiendo cálculo
    // Aquí repetimos cálculo casteando a float para seguridad
    float3 inputYCrCb = RGBtoYCrCb(InputF);
    float3 keyYCrCb = RGBtoYCrCb(KeyColorF);

    float chromaDist = distance(inputYCrCb.yz, keyYCrCb.yz);
    float baseAlpha = smoothstep((float)Threshold, (float)Threshold + (float)Softness, chromaDist);

    float spillVal = (1.0 - baseAlpha) * (float)DespillAmount;
    float3 lumaColor = float3(inputYCrCb.x, inputYCrCb.x, inputYCrCb.x);

    // Salida casteada de nuevo a half
    OutRGB = (half3)lerp(InputF, lumaColor, spillVal);
    OutAlpha = (half)baseAlpha;
}