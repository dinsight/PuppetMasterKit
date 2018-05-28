void main()
{
    vec2 coord = (v_tex_coord - 0.5) * 2.0;

    if (length(coord) > 1.0) {
        discard;
    }

    vec3 diffuseColor = SKDefaultShading().rgb;

    vec3 lightPosition = normalize(vec3(-1.0, 1.0, 1.0));

    float z = sqrt(1.0 - coord.x * coord.x - coord.y * coord.y);
    vec3 normal = normalize(vec3(coord.x, coord.y, z));

    float shadow = max(0.0, dot(normal, lightPosition));

    diffuseColor *= shadow;

    gl_FragColor = vec4(diffuseColor, 1.0);
}