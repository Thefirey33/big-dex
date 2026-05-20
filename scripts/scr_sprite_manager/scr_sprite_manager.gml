// I don't want bad scaling. So we're doing this.
#macro scr_sprite_manager_texture_size 64

global.sprite_collection = ds_map_create()

function scr_sprite_manager_sprite_load (filename) {
	show_debug_message(filename)
	var _temp_sprite = sprite_add(filename, 0, true, false, 0, 0)
	
	if not sprite_exists(_temp_sprite) {
		return
	}
	
	show_debug_message($"loading image {filename} and resizing to {scr_sprite_manager_texture_size}, {scr_sprite_manager_texture_size}...")
	// Resize the sprite to fit.
	var _surface = surface_create(scr_sprite_manager_texture_size, 
	scr_sprite_manager_texture_size)
	surface_set_target(_surface)
	draw_sprite_ext(
		_temp_sprite, 
		0, 
		0, 
		0, 
		scr_sprite_manager_texture_size / sprite_get_width(_temp_sprite), 
		scr_sprite_manager_texture_size / sprite_get_height(_temp_sprite), 
		0,
		c_white,
		255
	)
	
	var _new_sprite = sprite_create_from_surface(
		_surface, 
		0, 
		0, 
		scr_sprite_manager_texture_size, 
		scr_sprite_manager_texture_size, 
		true, 
		false, 
		0, 
		0
	)
	
	// It's very idiotic that I have to do this for EVERY sprite imported from the 
	// NikoDex API.
	// Delete the sprite so we don't leak memory.
	sprite_delete(_temp_sprite)
	surface_reset_target()
	surface_free(_surface)
	
	// Finally add the gawd dayum sprite
	ds_map_add(global.sprite_collection, filename_change_ext(filename, ""), _new_sprite)
}