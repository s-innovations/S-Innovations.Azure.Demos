interface ImplicitRequestOptions {
    responseType: string;
    prompt?: string;
    login_hint?: string;
    acr_values?: string;
    isSilence?: boolean;
}

export = ImplicitRequestOptions;