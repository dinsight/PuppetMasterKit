void main( void )
{
    vec3 bk = vec3( 0.9, 0.9, 0.9);
    vec2 coord = v_tex_coord;
    vec4 current = texture2D(u_texture, coord);
    float base_intensity = 0.23;

    float intensity = base_intensity + abs(sin(u_time)) * .4;

    float rw = 0.3;
    float rh = 0.3;
    float c = 0.15;
    vec2 center = vec2(0.5,0.5);
    float val = max((float)abs(coord.x-center.x) - rw, .0) * max((float)abs(coord.x-center.x) - rw, .0) +
                max((float)abs(coord.y-center.y) - rh, .0) * max((float)abs(coord.y-center.y) - rh, .0) ;

    if(val < c*c){
        gl_FragColor = vec4( current.rgb * intensity , current.a * intensity);
    } else {
        gl_FragColor = vec4(bk.rgb * base_intensity,0.3);
    }
}