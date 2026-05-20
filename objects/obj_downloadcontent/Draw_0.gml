var _sprites = ds_map_values_to_array(global.sprite_collection)
var _x_pos = 0
var _y_pos = 0

// Render all the noiks in a grid.
for (var i = 0; i < array_length(_sprites); i++) {
	draw_sprite(_sprites[i], 0, _x_pos, _y_pos)
	
	_x_pos += scr_sprite_manager_texture_size
	if (_x_pos > camera_get_view_width(camera_get_active()) - scr_sprite_manager_texture_size) {
		_x_pos = 0
		_y_pos += scr_sprite_manager_texture_size
	}
}

if network_busy {
	draw_text(0, 0, "Downloading...")
}