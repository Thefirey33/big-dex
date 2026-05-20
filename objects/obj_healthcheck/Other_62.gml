var _req_data = scr_nikodex_wrapper_extract_http_req("ping")

if scr_nikodex_wrapper_check(_req_data) {
	return
}

// Check if the NikoDex is alive and well.
// And not dead.

show_debug_message($"nikodex is okay... response in {_req_data}, moving forward to downloading images...")
instance_create_layer(0, 0, "Prepare", obj_downloadcontent)