shader_type canvas_item;

uniform sampler2D viewport : source_color;

vec2 pixelate(vec2 uv, float amount) {
    return round(uv*amount) / amount;
}

void fragment() {
	vec3 view = texture(viewport, pixelate(UV, 10)).rgb;
	// COLOR = vec4(UV.x, UV.y, 0.0f, 1.0f);
	COLOR = vec4(view, 1.0);
}
