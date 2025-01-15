shader_type canvas_item;

uniform vec2 viewportSize;
uniform sampler2D viewport1 : source_color;
uniform sampler2D viewport2 : source_color;
uniform sampler2D viewport3 : source_color;
uniform float borderThickness : hint_range(0, 10, 0.1);  // width of the split boder
uniform vec4 borderLineColor : source_color;       // color of the split border

uniform vec2 anchorA;
uniform vec2 anchorB;
uniform vec2 anchorC;
uniform vec2 origin;

float triangleSign(vec2 p1, vec2 p2, vec2 p3) {
    return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
}

//Implentation of this algorithm: https://stackoverflow.com/questions/2049582/how-to-determine-if-a-point-is-in-a-2d-triangle
bool isInsideTriangle(vec2 v1, vec2 v2, vec2 v3, vec2 point) {
    float d1,d2,d3;
    bool has_neg, has_pos;

    d1 = triangleSign(point, v1, v2);
    d2 = triangleSign(point, v2, v3);
    d3 = triangleSign(point, v3, v1);

    has_neg = (d1 < 0.0) || (d2 < 0.0) || (d3 < 0.0);
    has_pos = (d1 > 0.0) || (d2 > 0.0) || (d3 > 0.0);

    return !(has_neg && has_pos);
}

vec2 pixelate(vec2 uv, float amount) {
    return round(uv*amount) / amount;
}

float drawLine(vec2 a, vec2 b, vec2 p) {
    vec4 fragment;
    vec2 pa = p - a;
    vec2 ba = b - a;

    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    float d = length(pa - ba * h);

    return step(d, 1.0 / viewportSize.x);
}

vec4 drawBorders(vec4 fragment, vec2 uv) {
    fragment = mix(fragment, borderLineColor, drawLine(anchorA, origin, uv));
    fragment = mix(fragment, borderLineColor, drawLine(anchorB, origin, uv));
    fragment = mix(fragment, borderLineColor, drawLine(anchorC, origin, uv));
    return fragment;
}

vec4 threeCamera(vec2 uv) {
    vec4 fragment;
    vec3 view1 = texture(viewport1, pixelate(uv, 150)).rgb;
    vec3 view2 = texture(viewport2, pixelate(uv, 150)).rgb;
    vec3 view3 = texture(viewport3, pixelate(uv, 150)).rgb;

    // Create an array of edge points, going clockwise starting in the top left corner, corner inclusive
    vec2 corners[7] = {vec2(0.0, 0.0), anchorA, vec2(1.0, 0.0), anchorB, vec2(1.0, 1.0), anchorC, vec2(0.0, 1.0)};
    
    // An array tracking which triangle corresponds to each viewport
    int viewTriangles[7] = int[] (0, 1, 1, 2, 2, 0, 0);

    //Draw our triangles and fill each with the corresponding view
    fragment = vec4(0.5,0.0,0.0,1.0);
    for (int idx = 0; idx < corners.length(); idx++) {
        int nextIdx = (idx + 1) % corners.length();
        if (isInsideTriangle(origin, corners[idx], corners[nextIdx], uv)) {
            switch (viewTriangles[idx]%viewTriangles.length()) {
                case 0:
                    fragment = vec4(view1, 1.0);
                    break;
                case 1:
                    fragment = vec4(view2, 1.0);
                    break;
                case 2:
                    fragment = vec4(view3, 1.0);
                    break;
                default:
                    fragment = vec4(view1, 1.0);
                    break;
            }
            // switch (viewTriangles[idx]%viewTriangles.length()) {
            //     case 0:
            //         fragment = vec4(1.0,0.0,0.0,0.5);
            //         break;
            //     case 1:
            //         fragment = vec4(0.0,1.0,0.0,0.5);
            //         break;
            //     case 2:
            //         fragment = vec4(0.0,0.0,1.0,0.5);
            //         break;
            //     default:
            //         fragment = vec4(1.0,0.0,1.0,0.5);
            //         break;
            // }
            // if(idx==2) {
            //     fragment = vec4(0.5,0.0,0.5,1.0);
            // }
            //fragment = vec4(float(idx)*0.1,0.0,1.0-float(idx)*.1,1.0);
            break;
        }
    }
    //return fragment;
    return drawBorders(fragment, uv);
}

void fragment() {
    COLOR = threeCamera(UV);
}