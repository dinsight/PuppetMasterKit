

void main( void )
{
    vec2 coord = v_tex_coord;
    vec3 bk = vec3( 0.9, 0.9, 0.9);

    float intensity = 0.23;
    float opacity = 0.5;
    float rw = 0.3;
    float rh = 0.3;
    float c = 0.15;
    vec2 center = vec2(0.5,0.5);
    float val = max((float)abs(coord.x-center.x) - rw, .0) * max((float)abs(coord.x-center.x) - rw, .0) +
                max((float)abs(coord.y-center.y) - rh, .0) * max((float)abs(coord.y-center.y) - rh, .0) ;


    float delta = 0.03;
    float val2 = max((float)abs(coord.x-center.x) - (rw + delta), .0) * max((float)abs(coord.x-center.x) - (rw + delta), .0) +
                 max((float)abs(coord.y-center.y) - (rh + delta), .0) * max((float)abs(coord.y-center.y) - (rh + delta), .0) ;

    
    if(val >= c*c){
        float dst = val/val2;
        intensity = intensity * dst;
    }

    gl_FragColor = vec4( bk * intensity , opacity );
}