shader_type canvas_item;

varying vec2 world_position;

vec2 rotate_uv(vec2 uv, vec2 pivot, float angle) {
    float s = sin(angle);
    float c = cos(angle);
    mat2 rot_matrix = mat2(vec2(c, s), vec2(-s, c));
    
    uv -= pivot;
    uv = rot_matrix * uv;
    uv += pivot;
    return uv;
}

void vertex() {
	world_position = (MODEL_MATRIX * vec4(VERTEX, 0.0, 1.0)).xy;
}

void fragment() {
	vec2 grid = fract(world_position / 8.0);
	grid = rotate_uv(world_position, vec2(0.5), PI / 4.0);
	
	grid.x += TIME * 5.0;
	
	COLOR = vec4(1.0, 1.0, 1.0, sin(grid.x * 1.0) > 0.0 ? 1.0 : 0.0);
}