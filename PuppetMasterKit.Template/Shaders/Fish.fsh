
void main() {
    
    // Set up some animation parameters for the waveform
    
    float speed = u_time * 0.45;
    float frequency = 19.0;
    float intensity = 0.06;
    
    // Get the coordinate for the target pixel
    vec2 coord = v_tex_coord;
    
    
    // Rather than the original pixel color, using the offset target pixel
    vec4 targetPixelColor = vec4(1,1,1,0);

    vec4 currTexture  = texture2D(u_texture, coord);

    if(currTexture.a > 0)
        gl_FragColor = vec4(1,1,1,0);
    else
        gl_FragColor = currTexture;
}
