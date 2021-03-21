
void main( void )
{
    vec2 coord = v_tex_coord;
    vec4 current = texture2D(u_texture, coord);

    if(current[0]==current[1] && current[1]==current[2] && current[3] > 0){
        float factor = abs(sin(u_time));
        gl_FragColor = vec4(0.766, 0.448, 0.154, factor);
    }
    gl_FragColor = current;
}