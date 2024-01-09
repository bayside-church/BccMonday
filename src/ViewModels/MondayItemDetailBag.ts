
export type MondayItemDetailBag = {
    name: string;
    createdAt: string;
    updates: MondayUpdateBag[]
}

export type MondayUpdateBag = {
    id: number | string;
    createdAt: string;
    creatorName: string;
    textBody: string;
    replies: MondayUpdateBag[] | null;
    files: MondayAssetBag[] | null
}

export type MondayAssetBag = {
    id: number | string;
    name: string;
    publicUrl: string;
}

export type ColumnValue = {
    id: number | string;
    text: string;
    type: ColumnType;
    value: string;
}

export enum ColumnType {
    auto_number,
    board_relation,
    button,
    checkbox,
    color_picker,
    country,
    creation_log,
    date,
    dependency,
    doc,
    dropdown,
    email,
    file,
    formula,
    hour,
    item_assignees,
    item_id,
    last_updated,
    link,
    location,
    long_text,
    mirror,
    name,
    numbers,
    people,
    phone,
    progress,
    rating,
    status,
    subtasks,
    tags,
    team,
    text,
    timeline,
    time_tracking,
    vote,
    week,
    world_clock,
    unsupported,
}