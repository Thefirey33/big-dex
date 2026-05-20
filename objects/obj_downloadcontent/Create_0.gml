// This is the general state of this object.
// This object, prepares the content by downloading a random set of Nikos from the API,
// So as the game asks questions, the API can dynamically be interacted with.

function obj_prepare_rnoik_download () {
	request_id = scr_nikodex_wrapper_get_image(
		floor(random_range(0, global.noik_count))
	)
}

enum obj_prepare_states {
	recieving_niko_count,
	downloading_image,
	downloading_text
}

content_preparation_phase = obj_prepare_states.recieving_niko_count
request_id = scr_nikodex_wrapper_get_count()
noik_count = ds_list_size(global.noik_collection)
network_busy = false