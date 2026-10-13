shader_type canvas_item;

varying vec2 world_position;

void vertex() {
	world_position = (MODEL_MATRIX * vec4(VERTEX, 0.0, 1.0)).xy;
}

void fragment() {
	vec2 grid = fract(world_position / 16.0);
	
	if((grid.x < 0.5 && grid.y < 0.5) || (grid.x > 0.5 && grid.y > 0.5))
	{
		COLOR = vec4(0.0, 0.0078, 0.1244, 1.0);
	}else
	{
		COLOR = vec4(0.0, 0.0314, 0.1647, 1.0);
	}
}