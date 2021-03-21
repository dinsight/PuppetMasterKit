
void main( void )
{
    float distanceFromTrunksBase = abs(v_tex_coord[1]); 
    float maxDivergence = mix(0.0,1.0,distanceFromTrunksBase)*0.038;
    float factor = sin(u_time * 2.0 + 1.3 + v_tex_coord[0]*5.0);
    vec2 deltaUV = vec2(maxDivergence * factor, 0);
    
    gl_FragColor = texture2D(u_texture, v_tex_coord + deltaUV); //6
}