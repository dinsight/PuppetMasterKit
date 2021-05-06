

void main( void )
{
    float pi = 3.14159265;
    vec2 coord = v_tex_coord;
    //vec4 current = texture2D(u_texture, coord);
    vec3 bk = vec3( 0.54, 0.71, 0.84);
    
    float waves_count = 14;
    float t = atan(coord.x,coord.y);
    float d = sin(pi/4+t) * length(coord.xy);
    

    float start = sin(u_time*0.7);
    float intensity = 0.1 + abs(sin(start+d*waves_count)) * 0.15;

    float rw = 0.35;
    float rh = 0.38;
    float c = 0.05;
    vec2 center = vec2(0.5,0.5);
    float val = max((float)abs(coord.x-center.x) - rw, .0) * max((float)abs(coord.x-center.x) - rw, .0) +
                max((float)abs(coord.y-center.y) - rh, .0) * max((float)abs(coord.y-center.y) - rh, .0) ;
    float dst = (val-c*c)/0.01;

    intensity = intensity * dst;

    //https://www.geeksforgeeks.org/check-if-a-point-is-inside-outside-or-on-the-ellipse/
    float h = 0.5;
    float k = 1;
    float a = 1;
    float b = 0.2;

    if( ((coord.x-h)*(coord.x-h))/(a*a) + (coord.y-k)*(coord.y-k)/(b*b) <=1 ){
        intensity += 0.16;
    }

    gl_FragColor = vec4( bk * intensity, 0.25 );
}