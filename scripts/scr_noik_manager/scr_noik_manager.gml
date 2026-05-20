global.noik_collection = ds_list_create()
global.noik_count = 0
random_set_seed(date_current_datetime())
	
/**
 * Noik Object.
 * @param {string} _name Description
 * @param {real} _noik_id Description
 * @param {Asset.GMSprite} _sprite_id Description
 */
function obj_noik (_name, _noik_id, _sprite_id) constructor {
	name = _name
	noik_id = _noik_id
	sprite_id = _sprite_id
	
	/// @desc Render this Noik to the screen.
	/// @param {real} _x X Position.
	/// @param {real} _y Y Position.
	function render_this_noik (_x, _y) {
		draw_sprite(sprite_id, 0, _x, _y)
	}
	
}