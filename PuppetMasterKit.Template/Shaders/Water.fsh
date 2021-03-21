void main() {
    float sp = 3.0;
    float strn = 2.0;
    float frq = 5.0;

    // bring both speed and strength into the kinds of ranges we need for this effect
    float speed = u_time * sp * 0.05;
    float strength = strn / 100.0;

    // take a copy of the current texture coordinate so we can modify it
    vec2 coord = v_tex_coord;

    // offset the coordinate by a small amount in each direction, based on wave frequency and wave strength
    coord.x += sin((coord.x + speed) * frq) * strength;
    coord.y += cos((coord.y + speed) * frq) * strength;

    // use the color at the offset location for our new pixel color
    gl_FragColor = texture2D(u_texture, coord) * v_color_mix.a;
}