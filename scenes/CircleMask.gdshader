shader_type canvas_item;

uniform float mask_radius : hint_range(0.0, 1.0) = 1.0;

void fragment() {
    vec2 uv = UV * 2.0 - 1.0; // 将 UV 坐标转换到 [-1,1] 区间
    float dist = length(uv);  // 计算当前像素到中心的距离
    
    // 计算透明度，使得超出 mask_radius 的部分透明
    float alpha = dist <= mask_radius ? 1.0 : 0.0;
    
    vec4 tex_color = texture(TEXTURE, UV);
    COLOR = vec4(tex_color.rgb, tex_color.a * alpha);
}
