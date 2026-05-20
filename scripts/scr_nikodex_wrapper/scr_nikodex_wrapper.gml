#macro scr_nikodex_wrapper_other_request "other_req"
#macro scr_nikodex_wrapper_error "error"
#macro scr_nikodex_max_noik 100

/**
 * This will extract the HTTP information from the request made.
 * @param {string} request_id The Request ID.
 */
function scr_nikodex_wrapper_extract_http_req (request_id) {
	
	if string_pos(request_id, ds_map_find_value(async_load, "url")) == 0
		return scr_nikodex_wrapper_other_request
	
	return ds_map_find_value(async_load, "result")
}


/**
 * This will extract the HTTP information from the request made.
 * @param {array} request_id The Request ID.
 */
function scr_nikodex_wrapper_extract_http_req_a (request_id) {
	
	var _check = true
	
	for (var i = 0; i < array_length(request_id); i++) {
		if string_pos(request_id[i], ds_map_find_value(async_load, "url")) == 0
			_check = false
	}
	
	if _check
		return scr_nikodex_wrapper_other_request
	
	return ds_map_find_value(async_load, "result")
}

/**
 * This checks if the request that was made is not an error-ed request.
 * @param {string} _request_data The recieved request data.
 * @returns {bool} If the request is valid.
 */
function scr_nikodex_wrapper_check (_request_data) {
	return (_request_data == scr_nikodex_wrapper_other_request or _request_data == scr_nikodex_wrapper_error)
}

/**
 * Check if the download has finished.
 * @returns {bool} Returns true if the download has finished.
 */
function scr_nikodex_wrapper_check_connection () {
	return ds_map_find_value(async_load, "status") == 0
}

/**
 * This just pings the NikoDex API to make sure it's running.
 * @returns {real} The HTTP Request ID.
 */
function scr_nikodex_wrapper_ping () {
	return http_get(scr_make_api_s("ping"))
}

/**
 * This just pings the NikoDex API to get the Noik count.
 * @returns {real} The HTTP Request ID.
 */
function scr_nikodex_wrapper_get_count () {
	return http_get(scr_make_api(["data", "count"]))
}

/**
 * This downloads an image to the local image repository.
 * @param {real} _id Description
 * @returns {real} Description
 */
function scr_nikodex_wrapper_get_image (_id) {
	return http_get_file(scr_make_api_s($"image?id={_id}"), _id)
}
