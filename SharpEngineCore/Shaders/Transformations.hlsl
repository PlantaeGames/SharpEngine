float4 TransformWorld(float4 modalCoords,
    float4 worldPos, float4 angles, float4 scale)
{
    float4 coordinates = modalCoords;
    coordinates.w = 1;

    // rotations
    float3 rotations = 0;
    rotations.x = angles.x;
    rotations.y = angles.y;
    rotations.z = angles.z;

    // rotation
    //  rotation rotation
    row_major matrix zRotationMatrix = 0;
    zRotationMatrix[1][1] = cos(radians(rotations.z));
    zRotationMatrix[1][2] = -sin(radians(rotations.z));
    zRotationMatrix[2][1] = sin(radians(rotations.z));
    zRotationMatrix[2][2] = cos(radians(rotations.z));

    zRotationMatrix[0][0] = 1;
    zRotationMatrix[3][3] = 1;

    // x rotation
    row_major matrix xRotationMatrix = 0;
    xRotationMatrix[0][0] = cos(radians(rotations.x));
    xRotationMatrix[0][2] = sin(radians(rotations.x));
    xRotationMatrix[2][0] = -sin(radians(rotations.x));
    xRotationMatrix[2][2] = cos(radians(rotations.x));

    xRotationMatrix[1][1] = 1;
    xRotationMatrix[3][3] = 1;

    // y rotation
    row_major matrix yRotationMatrix = 0;
    yRotationMatrix[0][0] = cos(radians(rotations.y));
    yRotationMatrix[0][1] = -sin(radians(rotations.y));
    yRotationMatrix[1][0] = sin(radians(rotations.y));
    yRotationMatrix[1][1] = cos(radians(rotations.y));

    yRotationMatrix[2][2] = 1;
    yRotationMatrix[3][3] = 1;

    coordinates = mul(xRotationMatrix, coordinates);
    coordinates = mul(yRotationMatrix, coordinates);
    coordinates = mul(zRotationMatrix, coordinates);

    // sclaing
    row_major matrix scalingMatrix = 0;
    scalingMatrix[0][0] = scale.x;
    scalingMatrix[1][1] = scale.y;
    scalingMatrix[2][2] = scale.z;
    scalingMatrix[3][3] = 1;

    coordinates = mul(scalingMatrix, coordinates);

    // translation
    row_major matrix translationMatrix = 0;
    translationMatrix[0][3] = worldPos.x;
    translationMatrix[1][3] = worldPos.y;
    translationMatrix[2][3] = worldPos.z;

    translationMatrix[0][0] = 1;
    translationMatrix[1][1] = 1;
    translationMatrix[2][2] = 1;
    translationMatrix[3][3] = 1;

    coordinates = mul(translationMatrix, coordinates);

    coordinates.w = 1;
    return coordinates;
}

float4 TransformCameraView(float4 worldCoords,
    float4 camPosition, float4 camAngles)
{
    float4 coordinates = worldCoords;
    coordinates.w = 0;

    // translation
    coordinates.x -= camPosition.x;
    coordinates.y -= camPosition.y;
    coordinates.z -= camPosition.z;

    // rotations
    float4 rotations = 0;
    rotations.x = -camAngles.x;
    rotations.y = -camAngles.y;
    rotations.z = -camAngles.z;

    // z rotation
    row_major matrix zRotationMatrix = 0;
    zRotationMatrix[1][1] = cos(radians(rotations.z));
    zRotationMatrix[1][2] = -sin(radians(rotations.z));
    zRotationMatrix[2][1] = sin(radians(rotations.z));
    zRotationMatrix[2][2] = cos(radians(rotations.z));

    zRotationMatrix[0][0] = 1;
    zRotationMatrix[3][3] = 1;

    // x rotation
    row_major matrix xRotationMatrix = 0;
    xRotationMatrix[0][0] = cos(radians(rotations.x));
    xRotationMatrix[0][2] = sin(radians(rotations.x));
    xRotationMatrix[2][0] = -sin(radians(rotations.x));
    xRotationMatrix[2][2] = cos(radians(rotations.x));

    xRotationMatrix[1][1] = 1;
    xRotationMatrix[3][3] = 1;

    // y rotation
    row_major matrix yRotationMatrix = 0;
    yRotationMatrix[0][0] = cos(radians(rotations.y));
    yRotationMatrix[0][1] = -sin(radians(rotations.y));
    yRotationMatrix[1][0] = sin(radians(rotations.y));
    yRotationMatrix[1][1] = cos(radians(rotations.y));

    yRotationMatrix[2][2] = 1;
    yRotationMatrix[3][3] = 1;

    coordinates = mul(xRotationMatrix, coordinates);
    coordinates = mul(yRotationMatrix, coordinates);
    coordinates = mul(zRotationMatrix, coordinates);

    coordinates.w = 1;
    return coordinates;
}

float4 TransformPerspective(float4 viewCoords,
    float aspectRatio, float fov, float nearPlane, float farPlane)
{
    float4 coordinates = viewCoords;
    coordinates.w = 1;

    // perspective transformation  
    // creating perspective transformaopm matrix
    row_major matrix perspectiveMatrix = 0;
    perspectiveMatrix[0][0] = aspectRatio * 1 / tan(radians(fov * 0.5));
    perspectiveMatrix[1][1] = 1 / tan(radians(fov * 0.5));
    perspectiveMatrix[2][2] = (farPlane / (farPlane - nearPlane));
    perspectiveMatrix[2][3] = (-farPlane * nearPlane) / (farPlane - nearPlane);
    perspectiveMatrix[3][2] = 1; // saving original z value;

    // applying perspective transformation
    float4 result = mul(perspectiveMatrix, coordinates);

    return result;
}

float4 TransformWorldViewPerspective(float4 vertexPos, 
    float4 worldPos, float4 angles, float4 scale,
    float4 camPos, float4 camAngles,
    float aspectRatio, float fov, float nearPlane, float farPlane)
{
    float4 worldCoords = TransformWorld(vertexPos, worldPos, angles, scale);
    float4 viewCoords = TransformCameraView(worldCoords, camPos, camAngles);
    float4 projectionCoords = TransformPerspective(
                                    viewCoords, aspectRatio, fov, nearPlane, farPlane);

    return projectionCoords;
}

float4 TransformNormal(float4 normal,
    float4 angles)
{
    float4 coordinates = normal;
    coordinates.w = 0;

    float4 rotations = 0;
    rotations.x = angles.x;
    rotations.y = angles.y;
    rotations.z = angles.z;

    row_major matrix zRotationMatrix = 0;
    zRotationMatrix[1][1] = cos(radians(rotations.z));
    zRotationMatrix[1][2] = -sin(radians(rotations.z));
    zRotationMatrix[2][1] = sin(radians(rotations.z));
    zRotationMatrix[2][2] = cos(radians(rotations.z));

    zRotationMatrix[0][0] = 1;
    zRotationMatrix[3][3] = 1;

    // x rotation
    row_major matrix xRotationMatrix = 0;
    xRotationMatrix[0][0] = cos(radians(rotations.x));
    xRotationMatrix[0][2] = sin(radians(rotations.x));
    xRotationMatrix[2][0] = -sin(radians(rotations.x));
    xRotationMatrix[2][2] = cos(radians(rotations.x));

    xRotationMatrix[1][1] = 1;
    xRotationMatrix[3][3] = 1;

    // y rotation
    row_major matrix yRotationMatrix = 0;
    yRotationMatrix[0][0] = cos(radians(rotations.y));
    yRotationMatrix[0][1] = -sin(radians(rotations.y));
    yRotationMatrix[1][0] = sin(radians(rotations.y));
    yRotationMatrix[1][1] = cos(radians(rotations.y));

    yRotationMatrix[2][2] = 1;
    yRotationMatrix[3][3] = 1;

    coordinates = mul(xRotationMatrix, coordinates);
    coordinates = mul(yRotationMatrix, coordinates);
    coordinates = mul(zRotationMatrix, coordinates);

    coordinates.w = 0;
    return coordinates;
}

float4 TransformOrthogonal(float4 viewCoords, float4 scale)
{
    float4 coordinates = viewCoords;
    coordinates.w = 1;

    row_major matrix orthogonalMatrix = 0;
    orthogonalMatrix[0][0] = 2 / scale.x;
    orthogonalMatrix[1][1] = 2 / scale.y;
    orthogonalMatrix[2][2] = -2 / scale.z;

    orthogonalMatrix[0][3] = - 1 / scale.x;
    orthogonalMatrix[1][3] = - 1 / scale.y;
    orthogonalMatrix[2][3] = - 1 / scale.z;

    orthogonalMatrix[3][3] = 1;

    coordinates = mul(orthogonalMatrix, coordinates);
    
    coordinates.z = saturate(coordinates.z);
    return coordinates;
}

float4 TransformLightView(float4 worldCoords, float4 lightCoords, float4 lightAngles)
{
    float4 coordinates = worldCoords;
    coordinates.w = 0;

    // translation
    coordinates.x -= lightCoords.x;
    coordinates.y -= lightCoords.y;
    coordinates.z -= lightCoords.z;

    // rotations
    float4 rotations = 0;
    rotations.x = -lightAngles.x;
    rotations.y = -lightAngles.y;
    rotations.z = -lightAngles.z;

    // z rotation
    row_major matrix zRotationMatrix = 0;
    zRotationMatrix[1][1] = cos(radians(rotations.z));
    zRotationMatrix[1][2] = -sin(radians(rotations.z));
    zRotationMatrix[2][1] = sin(radians(rotations.z));
    zRotationMatrix[2][2] = cos(radians(rotations.z));

    zRotationMatrix[0][0] = 1;
    zRotationMatrix[3][3] = 1;

    // x rotation
    row_major matrix xRotationMatrix = 0;
    xRotationMatrix[0][0] = cos(radians(rotations.x));
    xRotationMatrix[0][2] = sin(radians(rotations.x));
    xRotationMatrix[2][0] = -sin(radians(rotations.x));
    xRotationMatrix[2][2] = cos(radians(rotations.x));

    xRotationMatrix[1][1] = 1;
    xRotationMatrix[3][3] = 1;

    // y rotation
    row_major matrix yRotationMatrix = 0;
    yRotationMatrix[0][0] = cos(radians(rotations.y));
    yRotationMatrix[0][1] = -sin(radians(rotations.y));
    yRotationMatrix[1][0] = sin(radians(rotations.y));
    yRotationMatrix[1][1] = cos(radians(rotations.y));

    yRotationMatrix[2][2] = 1;
    yRotationMatrix[3][3] = 1;

    coordinates = mul(xRotationMatrix, coordinates);
    coordinates = mul(yRotationMatrix, coordinates);
    coordinates = mul(zRotationMatrix, coordinates);

    coordinates.w = 1;
    return coordinates;
}

float3 CalculateForwardDir(float4 angles)
{
    float3 coords = 0;
    
    // yaw = x
    // pitch = y
    // roll = z
    
    coords.x = cos(radians(angles.y)) * sin(radians(angles.x));
    coords.y = -sin(radians(angles.y));
    coords.z = cos(radians(angles.y)) * cos(radians(angles.x));
    
    coords = normalize(coords);

    return coords;
}

float3 CalculateRightDir(float4 angles)
{
    float3 coords = 0;
    
    // yaw = x
    // pitch = y
    // roll = z
    
    coords.x = cos(radians(angles.x));
    coords.y = 0;
    coords.z = -sin(radians(angles.x));
    
    coords = normalize(coords);
    
    return coords;
}

float3 CalculateUpDir(float4 angles)
{
    float3 coords = 0;
    
    // yaw = x
    // pitch = y
    // roll = z
    
    coords.x = sin(radians(angles.y)) * sin(radians(angles.x));
    coords.y = cos(radians(angles.y));
    coords.z = sin(radians(angles.y)) * cos(radians(angles.x));
    
    coords = normalize(coords);
    
    return coords;
}

float CalculateInverseLaw(float distance, float k)
{
    return 1 / (k + pow(distance, 2));
}

float CalculateEttenuation(
        float range,
        float distance, float eConst, float eLinear, float eExpo)
{
    float attenuation = eConst + 
                        eLinear * distance +
                        eExpo * distance * distance;
    
    attenuation = range / attenuation;

    return attenuation;
}