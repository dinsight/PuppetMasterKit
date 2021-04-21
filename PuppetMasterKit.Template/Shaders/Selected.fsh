void main( void )
{
    vec2 coord = v_tex_coord;
    vec4 current = texture2D(u_texture, coord);

    float intensity = 0.17;
    gl_FragColor = vec4(current[0]+intensity, current[1]+intensity, current[2]+intensity, current[3]);
    
}