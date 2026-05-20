#macro scr_configuration_filename "gameconfig.tdat"
#macro scr_configuration_nikodex_origin "nikodex_origin"
#macro scr_configuration_is_fullscreen "is_fullscreen"

global.configuration = ds_map_create()

/**
 * This loads the general config that the game has.
 */
function scr_configuration_load_default_configuration() {
	if ds_map_size(global.configuration) > 0
		ds_map_clear(global.configuration)
	
	ds_map_add(global.configuration, scr_configuration_nikodex_origin, "https://nikodex.net/api/")
	ds_map_add(global.configuration, scr_configuration_is_fullscreen, false)
}


/**
 * Save the configuration file.
 */
function scr_configuration_save_configuration() {
	show_debug_message("saving current configuration to disk...")
	
	scr_configuration_load_default_configuration()
	
	var _textfile = file_text_open_write(scr_configuration_filename)
	file_text_write_string(_textfile, ds_map_write(global.configuration))
	file_text_close(_textfile)
}

/**
 * Attempt to retrieve a value from the configuration.
 * @param {string} key The key of the value.
 */
function scr_configuration_get_key(key) {
	if ds_map_exists(global.configuration, key)
		return ds_map_find_value(global.configuration, key)
	throw $"key not found: {key}"
}


/**
 * Load the configuration file.
 */
function scr_configuration_load_configuration() {
	show_debug_message("loading current configuration to memory...")
	
	var _readcontents = scr_read_all_lines_from_file(scr_configuration_filename)
	ds_map_read(global.configuration, _readcontents)
}

if file_exists(scr_configuration_filename) {
	scr_configuration_load_configuration()
} else {
	scr_configuration_save_configuration()
}
