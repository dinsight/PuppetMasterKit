void main( void )
{
    float margin = 0.2;
    vec2 center = vec2(0.5,0.5);
    vec4 margin_color = vec4(0.85,0.90,0.45, 1);
    vec2 coord = v_tex_coord;
    vec4 current = texture2D(u_texture, coord);

    float dist = distance(center, coord.xy);
    //float intensity = 1-dist + max(abs(sin(u_time)), 0.2);
    //gl_FragColor = vec4(current.rgb * intensity, current.a);

    float intensity = abs(sin(u_time));
    gl_FragColor = vec4(margin_color.rgb * intensity, margin_color.a * intensity);
}