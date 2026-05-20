/// @desc This function joins the specified paths together to form a webpath.
/// @param {array} path_list The routes to create.
/// @returns {string} Routed web path.
function scr_make_web_path(path_list){
	return string_join_ext("/", path_list)
}

/**
 * Create API route.
 * @param {array} path_list The API route path to follow.
 * @returns {string} The NikoDex API route created.
 */
function scr_make_api (path_list) {
	return string_concat(
		scr_configuration_get_key(scr_configuration_nikodex_origin), scr_make_web_path(path_list)
	)
}

/**
 * Create API route.
 * @param {array} path_list The API route path to follow.
 * @returns {string} The NikoDex API route created.
 */
function scr_make_api_s (path_list) {
	return string_concat(
		scr_configuration_get_key(scr_configuration_nikodex_origin), path_list
	)
}