shader_type canvas_item;

varying vec2 world_position;

void vertex() {
	world_position = (MODEL_MATRIX * vec4(VERTEX, 0.0, 1.0)).xy;
}

void fragment() {
	vec2 grid = fract(world_position / 8.0);
	grid.y -= TIME;
	grid.y = fract(grid.y);
	
	if((grid.x + grid.y < 0.75) || (grid.x + grid.y > 1.25))
	{
		COLOR = vec4(1.0, 1.0, 1.0, 1.0);
	}else
	{
		COLOR = vec4(1.0, 1.0, 1.0, 0.0);
	}
}