struct InputVertex
{
    float4 position : POSITION;
    float4 normal : NORMAL;
    float4 color : COLOR;
    float4 textureCoord : TEXCOORD;
};

struct OutputVertex
{
    float4 position : SV_Position;
    float4 normal : NORMAL;
    float4 color : COLOR;
    float4 textureCoord : TEXCOORD;
    float4 camPosition : CAMPOSITION;
    float4 worldPos : WORLDPOS;
};

cbuffer Transform : register(b0)
{
    float4 Position;
    float4 Rotation;
    float4 Scale;
    float4 W;
};

cbuffer CamTransform : register(b1)
{
    float4 CamPosition;
    float4 CamRotation;
    float4 CamScale;
    float AspectRatio;
    float Fov;
    float NearPlane;
    float FarPlane;
}

OutputVertex main(InputVertex vertex)
{
    OutputVertex output = (OutputVertex) 0;

    float4 coordinates;
    coordinates.x = vertex.position.x;
    coordinates.y = vertex.position.y;
    coordinates.z = vertex.position.z;
    coordinates.w = 1;
    
    float3 rotations = 0;
    rotations.x = Rotation.x;
    rotations.y = Rotation.y;
    rotations.z = Rotation.z;
    
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
    scalingMatrix[0][0] = Scale.x;
    scalingMatrix[1][1] = Scale.y;
    scalingMatrix[2][2] = Scale.z;
    scalingMatrix[3][3] = 1;
    
    coordinates = mul(scalingMatrix, coordinates);
    
    // translation
    row_major matrix translationMatrix = 0;
    translationMatrix[0][3] = Position.x;
    translationMatrix[1][3] = Position.y;
    translationMatrix[2][3] = Position.z;
    
    translationMatrix[0][0] = 1;
    translationMatrix[1][1] = 1;
    translationMatrix[2][2] = 1;
    translationMatrix[3][3] = 1;
    
    coordinates = mul(translationMatrix, coordinates);
    
    output.worldPos = coordinates;
    
    // camera respective
    coordinates.x -= CamPosition.x;
    coordinates.y -= CamPosition.y;
    coordinates.z -= CamPosition.z;
    
    rotations.x -= CamRotation.x;
    rotations.y -= CamRotation.y;
    rotations.z -= CamRotation.z;
   
    // perspective transformation  
    // creating perspective transformaopm matrix
    row_major matrix perspectiveMatrix = 0;
    perspectiveMatrix[0][0] = AspectRatio * 1 / tan(radians(Fov * 0.5));
    perspectiveMatrix[1][1] = 1 / tan(radians(Fov * 0.5));
    perspectiveMatrix[2][2] = (FarPlane / (FarPlane - NearPlane));
    perspectiveMatrix[2][3] = (-FarPlane * NearPlane) / (FarPlane - NearPlane);
    perspectiveMatrix[3][2] = 1; // saving original z value;
    
    // applying perspective transformation
    float4 result = mul(perspectiveMatrix, coordinates);
    
    output.position = float4(result.x, result.y, result.z, result.w);

    output.color = vertex.color;
    output.normal = vertex.normal;
    output.normal.w = 1;
    output.camPosition = CamPosition;
    
    // transforming normal
    coordinates = output.normal;
    
    zRotationMatrix = 0;
    zRotationMatrix[1][1] = cos(radians(rotations.z));
    zRotationMatrix[1][2] = -sin(radians(rotations.z));
    zRotationMatrix[2][1] = sin(radians(rotations.z));
    zRotationMatrix[2][2] = cos(radians(rotations.z));
    
    zRotationMatrix[0][0] = 1;
    zRotationMatrix[3][3] = 1;
    
    // x rotation
    xRotationMatrix = 0;
    xRotationMatrix[0][0] = cos(radians(rotations.x));
    xRotationMatrix[0][2] = sin(radians(rotations.x));
    xRotationMatrix[2][0] = -sin(radians(rotations.x));
    xRotationMatrix[2][2] = cos(radians(rotations.x));
    
    xRotationMatrix[1][1] = 1;
    xRotationMatrix[3][3] = 1;
    
    // y rotation
    yRotationMatrix = 0;
    yRotationMatrix[0][0] = cos(radians(rotations.y));
    yRotationMatrix[0][1] = -sin(radians(rotations.y));
    yRotationMatrix[1][0] = sin(radians(rotations.y));
    yRotationMatrix[1][1] = cos(radians(rotations.y));
    
    yRotationMatrix[2][2] = 1;
    yRotationMatrix[3][3] = 1;
    
    coordinates = mul(xRotationMatrix, coordinates);
    coordinates = mul(yRotationMatrix, coordinates);
    coordinates = mul(zRotationMatrix, coordinates);
    
    output.normal = coordinates;
    
    output.normal += Position;
    output.normal.w = 1;
    
    return output;
};