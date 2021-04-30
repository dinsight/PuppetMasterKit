
void main( void )
{
    float pi = 3.14159265;
    vec2 coord = v_tex_coord;
    vec4 current = texture2D(u_texture, coord);
    vec3 bk = vec3( 0.54, 0.71, 0.84);
    
    float waves_count = 14;
    float t = atan(coord.x,coord.y);
    float d = sin(pi/4+t) * length(coord.xy);
    float dtl = distance(coord.xy, vec2(0,1));

    float start = sin(u_time*0.7);
    float intensity = 0.15 + abs(sin(start+d*waves_count)) * 0.1;// + (1-dtl)*0.2;

    //https://www.geeksforgeeks.org/check-if-a-point-is-inside-outside-or-on-the-ellipse/
    float h = 0.5;
    float k = 1;
    float a = 1;
    float b = 0.3;

    if( ((coord.x-h)*(coord.x-h))/(a*a) + (coord.y-k)*(coord.y-k)/(b*b) <=1 ){
        intensity += 0.08;
    } 

    gl_FragColor = vec4( bk * intensity, 0.35 );
}