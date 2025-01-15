shader_type canvas_item;

uniform vec2 viewportSize;
uniform sampler2D viewport1 : source_color;
uniform sampler2D viewport2 : source_color;
uniform float borderThickness : hint_range(0, 10, 0.1);  // width of the split boder
uniform vec4 borderLineColor : source_color;       // color of the split border

uniform float slope;
uniform vec2 origin;

// from https://stackoverflow.com/questions/15276454/is-it-possible-to-draw-line-thickness-in-a-fragment-shader
float distanceToLine(vec2 p1, vec2 p2, vec2 point) {
	float a = p1.y - p2.y;
	float b = p2.x - p1.x;
	return abs(a * point.x + b * point.y + p1.x * p2.y - p2.x * p1.y) / sqrt(a * a + b * b);
}

vec4 twoCamera(vec2 uv) {
    vec4 fragment;
    vec3 view1 = texture(viewport1, uv).rgb;
    vec3 view2 = texture(viewport2, uv).rgb;

    float width = viewportSize.x;
    float height = viewportSize.y;
   
    vec2 lineStart = vec2(0.0, height * ((origin.x - 0.0) * slope + origin.y));
	vec2 lineEnd = vec2(width, height * ((origin.x - 1.0) * slope + origin.y));

    float borderCurrentY = (origin.x - uv.x) * slope + origin.y;

    if(uv.y < borderCurrentY) {
        fragment = vec4(view1, 1.0);
    } else {
        fragment = vec4(view2, 1.0);
    }

    float distanceToBorder = distanceToLine(lineStart, lineEnd, vec2(uv.x * width, uv.y * height));
    if (distanceToBorder < borderThickness) {
        fragment = borderLineColor;
    }
    return fragment;
}

void fragment() {
    COLOR = twoCamera(UV);
}