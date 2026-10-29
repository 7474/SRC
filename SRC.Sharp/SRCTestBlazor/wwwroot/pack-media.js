window.packMedia = {
    create: (bytes, type) => URL.createObjectURL(new Blob([bytes], { type })),
    revoke: url => URL.revokeObjectURL(url)
};
